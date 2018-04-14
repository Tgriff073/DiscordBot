using Discord;
using Discord.Commands;
using Discord.Audio;
using Discord.Net;
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using VideoLibrary;
using System.Diagnostics;
using YoutubeExtractor;
using TagLib;
using PortableSteam;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Threading;
namespace EmoticonBot
{
    class Program
    {
        public static MusicQueue songs = new MusicQueue();
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;
        private IAudioClient ac;
        private IVoiceChannel channel;
      
        public static void Main(string[] args)
                => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection()
               .BuildServiceProvider();

            Console.WriteLine(client.LoginState.ToString());
            string token = ""; // Remember to keep this private!
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            Console.WriteLine("bot connected");
            
            Console.WriteLine(client.LoginState.ToString());
            client.MessageReceived += Bot_MessageReceived;
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
       
       





        private async Task Bot_MessageReceived(SocketMessage message)
        {
            var m = message as SocketUserMessage;
            if (m == null || m.Author.Id == 207432805406343168)
                return;
            if (message.Content.ToString().IndexOf(":") >= 0)
            {
                Console.WriteLine("{0} said: {1}", message.Author.Username, message.Content.ToString());
                string[] words = System.IO.File.ReadAllLines("C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\configs\\commands.txt");

                //help command displays help information for users, the information is read from the file commands.txt which is stored in the config folder for the bot
                if (message.Content.ToString().ToLower().Contains(":halp"))
                {
                    string help = "```Commands(Start all commands with ':'. Example-> :kappa):\n";
                    int counter = 0;
                    foreach (string word in words)
                    {
                        counter++;
                        help += word.Substring(1, word.IndexOf(" ") - 1) + ", ";
                        if (counter % 4 == 0)
                            help += "\n";
                    }//end of for loop
                    help = help.Remove(help.Length - 2, 1);
                    help = help.Insert(help.Length - 1, ".```");
                    System.Threading.Thread.Sleep(150);
                    await message.Channel.SendMessageAsync(help);
                    Console.WriteLine("help called");
                }//end halp

                //coinflip command, calls coinflip and returns the filepath to either a heads or a tails. The image is then sent to discord
                else if (message.Content.ToString().ToLower().Contains(":coinflip"))
                {

                    string image = coinFlip();


                    await message.Channel.SendFileAsync(image);
                }


                //summon command, calls the summon function which makes the bot join the voice channel of the person who called it. Also sets _vClient equal to the return IAudioClient from the join function
                else if (message.Content.ToString().ToLower().Contains(":summon"))
                {
                    summon(message);
                }
                //end summon

                //play command, will determine if user is trying to play a local file(sound bites etc.), a youtube link, and more types of media will be added later(soundcloud?)
                //
                else if (message.Content.ToString().ToLower().Contains(":play"))
                {

                    try
                    {
                        if(ac == null)
                        {
                            await message.Channel.SendMessageAsync("Please type :summon before attempting to play a song");
                            return;
                        }
                        string linkOrName = message.Content.ToString().Substring(message.Content.ToString().IndexOf(" ") + 1);

                        string rootPath = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\audio\\";
                        if (message.Content.IndexOf(" ") > -1)
                        {
                            if (RemoteFileExists(linkOrName))
                            {

                                if (linkOrName.ToLower().Contains("youtube"))
                                {
                                    Song song = downloadSong(linkOrName);

                                    //mp4ToMp3(song.filename + ".mp4", song.filename + ".mp3");

                                    if (song.name != " ")
                                    {
                                        
                                        await message.Channel.SendMessageAsync(String.Format("Now Playing: {0}" + "\n", song.name));
                                        songs.addToQueue(song, message);
                                        
                                        if (songs.isPlaying == false)
                                        {
                                            songs.isPlaying = true;
                                            System.Threading.Thread thread = new System.Threading.Thread(() => musicPlayer(songs));
                                            thread.Name = "Song player";
                                            thread.Start();
                                            
                                        }
                                    }
                                    else
                                    {
                                        await message.Channel.SendMessageAsync("```An error occured while trying to play the requested song```");
                                    }


                                }
                            }
                            
                            else
                            {
                                await message.Channel.SendMessageAsync("```Invalid link!```");
                            }

                        }
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch
                    {
                        await message.Channel.SendMessageAsync("```An error occured while trying to play/download your file. Either it could not be found or it was invalid```");
                    }
                    
                }


                //9gag link changer, a 9gag link has been detected so the link will be converted to a stripped down version depending on the type of media. If the media type is gif then it will straight send a gif rather than a link
                else if (message.Content.ToLower().Contains("9gag.com"))
                {
                    string msg = message.Content;
                    string basePath = "http://img-9gag-fun.9cache.com/photo/";
                    string final = " ";
                    string rest = msg.Substring(msg.IndexOf("gag/") + 4);
                    //parsing link to find appropiate base link
                    if (rest.IndexOf("?") >= 0)
                        rest = rest.Substring(0, rest.IndexOf("?"));

                    //media type is gif 
                    if (RemoteFileExists(basePath + rest + "_460sa.gif"))
                    {
                        final = basePath + rest + "_460sa.gif";
                    }

                    //media type is mp4
                    else if (RemoteFileExists(basePath + rest + "_460sv.mp4"))
                    {
                        final = basePath + rest + "_460sv.mp4";
                    }

                    //media type is webm
                    else if (RemoteFileExists(basePath + rest + "_460svwm.webm"))
                    {
                        final = basePath + rest + "_460svwm.webm";
                    }

                    //media type is jpg dont do anything
                    else if (RemoteFileExists(basePath + rest + "_700b.jpg"))
                    {
                        ;
                    }
                    ///as long as the file wasnt a jpg, send the new link/ or gif
                    if (final != " ")
                    {
                        await message.Channel.SendMessageAsync("9gag link detected, better link or gif here:");
                        await message.Channel.SendMessageAsync(final);
                    }
                }//end 9gag

                //pokemon command, this command calls on the pokeapi and grabs information about the request pokemon also saves the image of that pokemon 
                else if (message.Content.ToLower().Contains(":pokemon"))
                {
                    try
                    {
                        string pokeName = message.Content.ToLower().Substring(message.Content.ToLower().IndexOf(" ") + 1);
                        Pokemans pokeTemp = new Pokemans();
                        pokeTemp = Pokemans.getPokeInfo(pokeName);
                        WebClient webClient = new WebClient();
                        string imagePath = string.Format("C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\pokemon\\{0}.png", pokeTemp.id);
                        if (!System.IO.File.Exists(imagePath))
                        {
                            webClient.DownloadFile(pokeTemp.image, imagePath);
                        }
                        else
                            Console.WriteLine("image already exists in cache");
                        string lmessage = string.Format("```Name: {0}\nSpeed: {1}\nSpecial defense: {2}\nSpecial attack: {3}\nDefense: {4}\nAttack: {5}\nHP: {6}\nAbility 1: {7}\n", pokeTemp.name, pokeTemp.stats[0], pokeTemp.stats[1], pokeTemp.stats[2], pokeTemp.stats[3], pokeTemp.stats[4], pokeTemp.stats[5], pokeTemp.abilities[0]);

                        if (pokeTemp.abilities[1] != " ")
                        {
                            lmessage += "Ability 2: " + pokeTemp.abilities[1] + "\n";
                        }
                        if (pokeTemp.abilities[2] != " ")
                        {
                            lmessage += "Ability 3: " + pokeTemp.abilities[2] + "\n";
                        }
                        lmessage += "Type: " + pokeTemp.types[0] + "\n";
                        if (pokeTemp.types[1] != " ")
                        {
                            lmessage += "Sub-type: " + pokeTemp.types[1] + "\n";
                        }
                        lmessage += "```";
                        await message.Channel.SendFileAsync(imagePath);
                        System.Threading.Thread.Sleep(125);
                        await message.Channel.SendMessageAsync(lmessage);
                    }//end try (Pokemon error check)
                    catch
                    {
                        await message.Channel.SendMessageAsync("Couldn't find pokemon in the database!");
                        Console.WriteLine("Couldn't find pokemon in the database!");
                    }//end catch (Pokemon error check)

                }//end pokemon command

                //weather command, calls the weather function and sends information about the weather in the requseted location
                else if (message.Content.ToLower().Contains(":weather"))
                {
                    string location = message.Content.ToLower();
                    weather(location, message);
                }//end weather command

                //cards command, does a bunch of stuff with a deck of cards. Uses an online card api for the info api: http://deckofcardsapi.com
                else if (message.Content.ToLower().Contains(":cards"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Card cardDrawn = new Card();
                    //Console.WriteLine("this is a test");
                    WebClient webClient = new WebClient();
                    string deckId = "";
                    string rootPath = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\cards\\";
                    string jsonData = System.IO.File.ReadAllText(rootPath + "deck.txt");
                    XmlDocument deck = JsonConvert.DeserializeXmlNode(jsonData, "root");
                    string drawName = message.Author.Username;
                    if (!Game.isGameRunning)
                    {
                        deckId = deck.SelectSingleNode("/root/deck_id").InnerText;
                        jsonData = System.IO.File.ReadAllText(rootPath + "deck.txt");
                        webClient.DownloadFile("http://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1", (rootPath + "deck.txt"));
                    }
                    if (message.Content.ToLower().Contains("draw"))
                    {
                        Console.WriteLine(deckId);

                        if (!Game.isGameRunning)
                            cardDrawn = deckDraw(message, " ", drawName);
                        else
                            cardDrawn = deckDraw(message, Game.deckId, drawName);
                        await message.Channel.SendFileAsync(cardDrawn.imagePath);
                        Console.WriteLine(cardDrawn.valueS);
                    }
                    else if (message.Content.ToLower().Contains("new") && !Game.isGameRunning)
                    {
                        await message.Channel.SendMessageAsync("A new game of cards has been started, to participate in the game type :draw, to end the game type :cards quit");
                        Game.isGameRunning = true;
                        Game.deckId = deckId;
                        Console.WriteLine(deckId);

                    }
                    else if (message.Content.ToLower().Contains("quit") && Game.isGameRunning)
                    {
                        Game.isGameRunning = false;
                        if (Game.currentLeader != null)
                            await message.Channel.SendMessageAsync(string.Format("game has been ended by: {0}, the winner was: {1} with a {2} of {3}", message.Author.Username, Game.currentLeader, Game.currentHighCard.cardVal.ToLower(), Game.currentHighCard.suit.ToLower()));
                        Game.deckId = "";
                        Game.currentHigh = 0;
                        Game.currentHighCard = cardDrawn;
                        Game.currentLeader = "";

                    }
                }//end card game

                //turns my wemo on or off, only can be called by me
                else if (message.Content.ToLower().Contains(":wemo"))
                {
                    if (message.Author.Id == 140268276415594496)
                    {
                        Console.WriteLine("Calling wemo");
                        wemo(message.Content.ToLower());
                    }
                }//end wemo command

                //purge command, deletes the requested amount of messages
                else if (message.Content.ToLower().Contains(":purge"))
                {
                    //purgeMessages(message);
                }
                else if (message.Content.ToLower().Contains(":sort"))
                {
                   
                    Potter p = harrypotter();
                    if (p != null)
                    {
                        await message.Channel.SendMessageAsync(p.Message);
                        await message.Channel.SendFileAsync(p.Image);
                    }
                }
                else if(message.Content.ToLower().Contains(":screenshot"))
                {
                    string link = message.Content;
                    if (!(link.IndexOf(' ') > 0))
                        await message.Channel.SendMessageAsync("Failed to find inspect link from message make sure message format is :screenshot inspectlinkhere");
      
                    link = link.Substring(link.IndexOf(' '));
                    string id = link.Substring(link.IndexOf("%") + 1);
                    string status = String.Empty;
                    string final = String.Format("https://metjm.net/shared/screenshots-v5.php?cmd=request_new_link&inspect_link={0}", id);
                    string html = getEntry(final);
                    long fid = 0;
                    if (html.Contains("screen_age")) 
                    {
                        secReq re = JsonConvert.DeserializeObject<secReq>(html);
                        status = getEntry(String.Format("https://metjm.net/shared/screenshots-v5.php?cmd=request_screenshot_status&id={0}", re.result.screen_id));
                        fid = re.result.screen_id;
                    }
                   else 
                    {
                        req3 re2 = JsonConvert.DeserializeObject<req3>(html);
                        status = getEntry(String.Format("https://metjm.net/shared/screenshots-v5.php?cmd=request_screenshot_status&id={0}", re2.result.screen_id));
                        fid = re2.result.screen_id;
                    }
                    Thread.Sleep(1000);
                    Console.WriteLine(status);
                    Console.WriteLine(html);
                    Csitem request = JsonConvert.DeserializeObject<Csitem>(status);
                    if(request.result.image_url == null)
                    {
                        await message.Channel.SendMessageAsync("Invalid link");
                        return;
                    }
                    while(status.Contains("place_in_queue"))
                    {
                         await message.Channel.SendMessageAsync(String.Format("Waiting to get screenshot, you are number {0} in queue, checking again in 5 seconds", status.Substring(status.IndexOf("place_in_queue")+16, 1)));
                         Thread.Sleep(5001);
                         status = getEntry(String.Format("https://metjm.net/shared/screenshots-v5.php?cmd=request_screenshot_status&id={0}",fid));
                         
                    }
                    request = JsonConvert.DeserializeObject<Csitem>(status);
                    await message.Channel.SendMessageAsync(request.result.image_url);
                }
                else
                {
                    try
                    {
                        string lmessage = message.Content.ToLower();
                        string temp = "";
                        string tempF = "";
                        int index = 0;
                        int indexSpace = 0;
                        bool status = true;
                        while (status)
                        {

                            string[] lines = System.IO.File.ReadAllLines(@"C:\\Users\\thoma\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\configs\\commands.txt");
                            index = lmessage.IndexOf(":");
                            Console.WriteLine(index);
                            if (index >= 0)
                            {
                                lmessage = lmessage.Substring(index + 1);
                                //Console.WriteLine(message);
                                indexSpace = lmessage.IndexOf(" ");
                                if (indexSpace >= 0)
                                {
                                    temp = lmessage.Substring(0, indexSpace);
                                    // Console.WriteLine("displaying temp: {0}", temp);
                                }
                                else
                                {
                                    temp = lmessage;
                                    status = false;
                                }
                                foreach (string line in lines)
                                {
                                    tempF = line.Substring(1, line.IndexOf(" ") - 1);
                                    if (temp == tempF)
                                    {
                                        string ifile = line.Substring(line.IndexOf(" ") + 1);//image file name is pulled from config.txt
                                        Console.WriteLine(temp);
                                        await message.Channel.SendFileAsync(ifile);//send file specified from config.txt
                                    }
                                }//end of for loop
                                status = false;
                                lines = null;
                            }
                            else
                            {
                                status = false;
                                continue;
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("shit dont have no colon, no worries");//string parsing dun messed up, who knows what the problem could be
                    }
                }

            }//end of author check

        }//end of Bot_MessageReceived

         string getEntry(string url)
        {
            string html = string.Empty;

            string result = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            return html;
        }

        public void musicPlayer(MusicQueue q)
        {

            
            while (q.queue.Any())
            {
                play(q.queue.First());
                q.queue.RemoveAt(0);
            }
            q.isPlaying = false;

        }
        


        //summon method, takes the messageEventArgs of the bot to find the channelid of where the bot needs to go. Then the bot joins the channel sets the global voice client _vClient equal to the coressponding IAudioClient
        //will send an error message if an error occurs
        public async void summon(SocketMessage message)
        {
            channel = null;
            try
            {
                channel = channel ?? (message.Author as IGuildUser)?.VoiceChannel;
                if (channel == null) { await message.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

                // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
                ac = await channel.ConnectAsync();

            }
            catch
            {
                await message.Channel.SendMessageAsync("```An error occured while trying to connect to the channel\nPlease make sure you are currently connected to a voice channel\n```");
            }
        }




        //wemo method, sends an on or off command to the wemo depending on what the user input is. Should only be able to be called by me?
        private static void wemo(string option)
        {
            try
            {
                string command = "";
                if (option.Contains("on"))
                {
                    command = @"<?xml version=""1.0"" encoding=""utf - 8""?> <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""> <s:Body> <u:SetBinaryState xmlns:u=""urn:Belkin:service:basicevent:1""> <BinaryState>1</BinaryState> <Duration></Duration> <EndAction></EndAction> <UDN></UDN> </u:SetBinaryState> </s:Body> </s:Envelope>";
                    Console.WriteLine("sending on command to wemo");
                }
                else
                {
                    command = @"<?xml version=""1.0"" encoding=""utf - 8""?> <s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" s:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""> <s:Body> <u:SetBinaryState xmlns:u=""urn:Belkin:service:basicevent:1""> <BinaryState>0</BinaryState> <Duration></Duration> <EndAction></EndAction> <UDN></UDN> </u:SetBinaryState> </s:Body> </s:Envelope>";
                    Console.WriteLine("Sending off command to wemo");
                }
                string targetUrl = "http://192.168.1.185:49153/upnp/control/basicevent1";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(targetUrl);
                request.Method = "POST";
                request.Headers.Add("SOAPAction", "\"urn:Belkin:service:basicevent:1#SetBinaryState\"");
                request.ContentType = @"text/xml; charset=""utf-8""";
                request.KeepAlive = false;
                Byte[] bytes = UTF8Encoding.ASCII.GetBytes(command);
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    request.GetResponse();
                }
                request.Abort();
            }//end try
            catch
            {
                Console.WriteLine("Error occured while trying to interact with wemo\n");
            }
        }//end wemo

        public  Potter harrypotter()
        {
            string site = "http://www.h-o-g-w-a-r-t-s.com";
            string rootPath = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\images\\";
            string response = getEntry(site);
            Potter temp = new Potter();
            if (response.ToLower().Contains("hufflepuff"))
            {
                temp.setObj("You got Hufflepuff\n", rootPath + "huf.jpg", 0);
            }
            else if (response.ToLower().Contains("ravenclaw"))
            {
                temp.setObj("You got Ravenclaw\n", rootPath + "rav.jpg", 1);
            }
            else if (response.ToLower().Contains("slytherin"))
            {
                temp.setObj("You got Slytherin\n", rootPath + "sly.jpg", 1);
            }
            else if (response.ToLower().Contains("gryffindor"))
            {
                temp.setObj("You got Gryffindor\n", rootPath + "gryf.jpg", 1);
            }
            else
            {
                temp = null;
            }
            return temp;

        }
        //weather command, gets an xml document to find geographical information about the requested city. That information is then used to grab the weather of the requested city by using the 
        //wunderground weather api
        async void weather(string location, SocketMessage message)
        {
            if (location.IndexOf(" ") > 0)
            {
                Console.WriteLine("{0}", location.Substring(location.IndexOf(" ") + 1));
                location = location.Substring(location.IndexOf(" ") + 1);
            }
            XmlDocument xmlConditions = new XmlDocument();

            XmlDocument geoLocate = new XmlDocument();
            try
            {
                string city;
                string state;
                string conditions = "";
                string temperature = "";
                string windSpeed = "";
                string humidity = "";
                string address;
                string timeUpdated;
                WebClient webClient = new WebClient();
                string image = "";
                string imageSend = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\images\\weather\\test.png";
                geoLocate.Load(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", location));

                address = geoLocate.SelectSingleNode("/GeocodeResponse/result/formatted_address").InnerText;
                city = address.Substring(0, address.IndexOf(","));
                state = address.Substring(address.IndexOf(",") + 2, 2);
                xmlConditions.Load(string.Format("http://api.wunderground.com/api/21a9cf0b79275345/conditions/q/{0}/{1}.xml", state, city));
                conditions = xmlConditions.SelectSingleNode("/response/current_observation/weather").InnerText;
                temperature = xmlConditions.SelectSingleNode("/response/current_observation/temp_f").InnerText;
                humidity = xmlConditions.SelectSingleNode("/response/current_observation/relative_humidity").InnerText;
                windSpeed = xmlConditions.SelectSingleNode("/response/current_observation/wind_string").InnerText;
                image = xmlConditions.SelectSingleNode("/response/current_observation/icon_url").InnerText;
                timeUpdated = xmlConditions.SelectSingleNode("/response/current_observation/observation_time").InnerText;
                timeUpdated = timeUpdated.Substring(timeUpdated.IndexOf(",") + 1, timeUpdated.IndexOf("M") - timeUpdated.IndexOf(","));
                Console.WriteLine("{0}, {1}", city, state);
                string lmessage = string.Format("```Weather in {0}, {1} as of {6}:\nTemperature: {2}°F\nConditions: {5}\nHumidity: {3}\nWindspeed: {4}\n```", city, state, temperature, humidity, windSpeed, conditions, timeUpdated);
                webClient.DownloadFile(image, imageSend);
                await message.Channel.SendMessageAsync(lmessage);
                System.Threading.Thread.Sleep(100);
                await message.Channel.SendFileAsync(imageSend);
            }//end try block/weather
            catch//error occured most likely from the pesty xml blocks, dunno how to handle it correctly. Oh well
            {
                Console.WriteLine("{0} is not a valid location stop trolling the bot please.", location);//display error in command line
                location = char.ToUpper(location[0]) + location.Substring(1);//change first letter to capital letter
                string lmessage = string.Format("{0} is not a valid location stop trolling the bot please.", location);// create formatted message to be sent to discord client
                await message.Channel.SendMessageAsync(lmessage);//send formatted message
            }
        }//end weather

        //deckDraw method, does card stuff with deckofcardsapi
        public static Card deckDraw(SocketMessage message, string deckId, string dName)
        {
            Card cardSelected = new Card();
            JsonSerializer serializer = new JsonSerializer();
            Console.WriteLine("this is a test");
            WebClient webClient = new WebClient();
            int currentVal = 0;
            string rootPath = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\cards\\";

            if (deckId == " ")
            {
                webClient.DownloadFile("http://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1", (rootPath + "deck.txt"));
                string jsonData = System.IO.File.ReadAllText((rootPath + "deck.txt"));
                XmlDocument deck = JsonConvert.DeserializeXmlNode(jsonData, "root");
                deckId = deck.SelectSingleNode("/root/deck_id").InnerText;
            }
            webClient.DownloadFile(string.Format("http://deckofcardsapi.com/api/deck/{0}/draw/?count=1", deckId), (rootPath + "card.txt"));
            string cardData = System.IO.File.ReadAllText(rootPath + "card.txt");
            XmlDocument card = JsonConvert.DeserializeXmlNode(cardData, "root");
            string cardFinal = card.SelectSingleNode("/root/cards/code").InnerText;
            string image = card.SelectSingleNode("/root/cards/image").InnerText;
            string imagePath = rootPath + cardFinal + ".png";
            string cardSuit = card.SelectSingleNode("/root/cards/suit").InnerText;
            string cardVal = card.SelectSingleNode("/root/cards/value").InnerText;
            cardSelected.suit = cardSuit;
            cardSelected.imagePath = imagePath;
            cardSelected.valueS = cardFinal;
            cardSelected.cardVal = cardVal;


            if (!System.IO.File.Exists(imagePath))
                webClient.DownloadFile(image, imagePath);

            if (cardFinal.ToLower().Contains("a") || cardFinal.ToLower().Contains("j") || cardFinal.ToLower().Contains("q") || cardFinal.ToLower().Contains("k"))
            {
                if (cardFinal.ToLower().Contains("a"))
                    currentVal = 1;
                if (cardFinal.ToLower().Contains("j"))
                    currentVal = 11;
                if (cardFinal.ToLower().Contains("q"))
                    currentVal = 12;
                if (cardFinal.ToLower().Contains("k"))
                    currentVal = 13;
            }

            else
                currentVal = Convert.ToInt32(cardFinal[0]);

            cardSelected.value = currentVal;

            if (Game.isGameRunning)
            {
                if (currentVal > Game.currentHigh)
                {
                    Game.currentHigh = currentVal;
                    Game.currentLeader = message.Author.Username;
                    Game.currentHighCard = cardSelected;

                }
            }

            return cardSelected;

        }//end deckDraw


        //coinflip method, flips coins and stuff 
        public static string coinFlip()
        {
            string image = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\coin\\";
            int result;
            Random rnd = new Random();
            result = rnd.Next(1, 100);

            if (result >= 50)
                image += "heads.png";
            else
                image += "tails.png";
            return image;
        }//end coinflip



        //RemoteFileExists method, checks if there is a file at the requested url returns true if it exists and false otherwise
        public static bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }

        //plays a song and decodes it using naudio
        static void mp4ToMp3(string inputFile, string outputFile)
        {
            var process = Process.Start(new ProcessStartInfo
            { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
                FileName = "ffmpeg",
                Arguments = $"-i {"\"" + inputFile + "\""} " + // Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
                            $"-vn -f mp3 -ab 192k {"\"" + outputFile + "\""} ", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
                UseShellExecute = false,
                RedirectStandardOutput = true, // Capture the stdout of the process
                RedirectStandardError = true
            });
        }
        private Process CreateStream(string path)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i {"\"" + path + "\""} -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            return Process.Start(ffmpeg);
        }
        async void play(Song s)
        {
            if (System.IO.File.Exists(s.filename))
            {
                await client.SetGameAsync(s.name);
                var ffmpeg = CreateStream(s.filename);
                var output = ffmpeg.StandardOutput.BaseStream;
                var discord = ac.CreatePCMStream(AudioApplication.Mixed);
                await output.CopyToAsync(discord);
                await discord.FlushAsync();
            }//end file check
            else
                Console.WriteLine("File does not exist");

        }
        Song downloadSong(string vidLink)
        {
            Song song = new Song();
            string id = vidLink.Substring(vidLink.IndexOf("v=")+2);
            string link = "https://youtube2mp3.me/@api/json/mp3/";
            string final = link + id;
            try
            {

                string rootPath = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\audio\\";
                
                JsonSerializer serializer = new JsonSerializer();
                
                songIncomp downloaded = new songIncomp();

                WebClient wc = new WebClient();
               
                string jsonData = getEntry(string.Format("http://www.youtubeinmp3.com/fetch/?format=json&video=http://www.youtube.com/watch?v={0}", getId(vidLink)));
                downloaded = JsonConvert.DeserializeObject<songIncomp>(jsonData);
                song.name = downloaded.title;
                song.setFilename();
                song.link = downloaded.link;
                song.duration = downloaded.length;
               
                if (!System.IO.File.Exists(song.filename))
                    wc.DownloadFile(song.link, song.filename);
                else
                    Console.WriteLine("Song already exists in cache, not redownloading\n");
                return song;
            }
            catch
            {
                Console.WriteLine("Erorr downloading the song fuck fuck fuck\n");
                return song;
            }
        }
        static string getId(string link)
        {
            return link.Substring(link.IndexOf("=") + 1);
        }
        static Song downloadVid(string link)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
            VideoInfo video = videoInfos
                .First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
            Song temp = new Song(video.Title, "0", " ");
            temp.song = new MemoryStream();
            if (!System.IO.File.Exists(Path.Combine(@"C:\Users\thoma\Documents\Visual Studio 2015\Projects\EmoticonBot\EmoticonBot\audio", video.Title + video.VideoExtension)))
            {
                var videoDownloader = new VideoDownloader(video, Path.Combine(@"C:\Users\thoma\Documents\Visual Studio 2015\Projects\EmoticonBot\EmoticonBot\audio\", video.Title + video.VideoExtension));
                videoDownloader.Execute();
                var file = TagLib.File.Create(Path.Combine(@"C:\Users\thoma\Documents\Visual Studio 2015\Projects\EmoticonBot\EmoticonBot\audio", video.Title + video.VideoExtension));
                //temp.duration = (file.Properties.Duration.Minutes * 60) + file.Properties.Duration.Seconds;
                return temp;
            }

            return temp;
        }
        static void fourToThree(ref Song temp)
        {
            try
            {
                var outputBuffer = new MemoryStream();
                var process = Process.Start(new ProcessStartInfo
                { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
                    FileName = "ffmpeg",
                    Arguments = $"-i {"\"" + temp.filename + ".mp4\""} " + // Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
                                "-f s161e -vn 48000 -ac 2 pipe:1", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true// Capture the stdout of the process

                });

                //await outputBuffer.FlushAsync();

                process.StandardOutput.BaseStream.CopyTo(temp.song);
                temp.song.Flush();
                temp.song.Seek(0, SeekOrigin.Begin);
                process.WaitForExit();
            }
            catch
            {
                Console.WriteLine("error occurred during conversion from mp4 to mp3");
            }

        }

    }//end class
    class Potter
    {
        string message;
        string image;
        int house;
        public void setObj(string m, string i, int h) { message = m; image = i; house = h; }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        public int House
        {
            get { return house; }
            set { house = value; }
        }
    }
    class MusicQueue
    {
        public bool isPlaying;
        public List<Song> queue = new List<Song>();
        string time;
        int count;
        
        public async void addToQueue(Song toAdd, SocketMessage message)
        {
            queue.Add(toAdd);
            count = queue.Count;
            int totalTime = 0;
            for (int i = 0; i < queue.Count; i++)
            {
                totalTime += Convert.ToInt32(queue[i].duration);
            }
            int mins = totalTime/60;
            if (queue.Count > 1)
            {
                time = String.Format("{0} Minute(s) and {1} second(s)", mins, totalTime % mins);
                await message.Channel.SendMessageAsync($"```{toAdd.name} is #{queue.Count} and will be play in approximately {time} ```");
            }
        }
        public bool sendAudio(Song song)
        {
            return true;
        }
        public void play()
        {
            isPlaying = true;
            while(queue.Any())
            {
                sendAudio(queue.First());
                queue.RemoveAt(0);
            }
            isPlaying = false;
        }
        public static void update()
        {

        }
        public Song pop()
        {
            Song temp = queue.First();
            if (queue.Count > 0)
            {
                queue.RemoveAt(0);
                count--;
            }
            return temp;
        }
        public MusicQueue()
        {
            isPlaying = false;
            queue = new List<Song>();
            time = "";
            count = 0;

        }
        //public void skip

    }
   
    
    

}//end namespace
