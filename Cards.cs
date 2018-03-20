using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoticonBot
{
    class Game
    {
        public static bool isGameRunning = false;
        public static Card currentHighCard;
        public static int currentHigh = 0;
        public static string currentLeader;
        public static string deckId;
    }
    class Card
    {

        public int value;
        public string cardVal;
        public string suit;
        public string valueS;
        public string imagePath;
    }
}
