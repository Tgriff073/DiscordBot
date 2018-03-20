using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
namespace EmoticonBot
{
    class Pokemans
    {
        public string name, image;
        public string id;
        public string[] stats = new string[6];
        public string[] types = new string[2] { " ", " " };
        public string[] abilities = new string[3] { " ", " ", " " };
        //gets pokemon info
        public static Pokemans getPokeInfo(string name)
        {
            Pokemans temp = new Pokemans();
            JsonSerializer serializer = new JsonSerializer();
            Console.WriteLine(name);
            WebClient webClient = new WebClient();
            string rootPath = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\pokemon\\";
            webClient.DownloadFile(string.Format("http://pokeapi.co/api/v2/pokemon/{0}/", name), (rootPath + "pokemon.txt"));
            Console.WriteLine("Successfully downloaded: {0}.txt", name);
            string jsonData = System.IO.File.ReadAllText((rootPath + "pokemon.txt"));
            XmlDocument pokemon = JsonConvert.DeserializeXmlNode(jsonData, "root");
            XmlElement xelRoot = pokemon.DocumentElement;
            XmlNodeList types = pokemon.SelectNodes(@"//root/types");
            XmlNodeList xmlNodesAbilities = pokemon.SelectNodes(@"//root/abilities");
            XmlNodeList statS = pokemon.SelectNodes(@"//root/stats");


            temp.name = pokemon.SelectSingleNode("/root/forms/name").InnerText;
            temp.id = pokemon.SelectSingleNode("/root/id").InnerText;
            temp.image = string.Format("http://pokeapi.co/media/sprites/pokemon/{0}.png", temp.id);
            int countA = 0;
            int countT = 0;
            int count = 0;

            foreach (XmlNode stat in statS)
            {

                temp.stats[count++] = stat.SelectSingleNode("base_stat").InnerText;
                Console.WriteLine("test: {0}", temp.stats[count - 1]);
            }
            Console.WriteLine("test: {0}", temp.stats[count - 1]);
            foreach (XmlNode xmNode in xmlNodesAbilities)
            {
                temp.abilities[countA++] = xmNode.SelectSingleNode("ability/name").InnerText;
            }
            foreach (XmlNode type in types)
            {
                temp.types[countT++] = type.SelectSingleNode("type/name").InnerText;
            }
            Console.WriteLine(temp.image);
            return temp;
        }
    }
}
