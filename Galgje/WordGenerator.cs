﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galgje
{
    public class WordGenerator
    {
        private static string[] words = new string[] 
        {
            "grafeem", "tjiftjaf", "maquette", "kitsch", "pochet", "convocaat", "jakkeren",
            "collaps", "zuivel", "cesium", "voyant", "spitten", "pancake", "gietlepel",
            "karwats", "dehydreren", "viswijf", "flater", "cretonne", "sennhut", "tichel",
            "wijten", "cadeau", "trotyl", "chopper", "pielen", "vigeren", "vrijuit", "dimorf",
            "kolchoz", "janhen", "plexus", "borium", "ontweien", "quiche", "ijverig", "mecenaat",
            "falset", "telexen", "hieruit", "femelaar", "cohesie", "exogeen", "plebejer", "opbouw",
            "zodiak", "volder", "vrezen", "convex", "verzenden", "ijstijd", "fetisj", "gerekt",
            "necrose", "conclaaf", "clipper", "poppetjes", "looikuip", "hinten", "inbreng", "arbitraal",
            "dewijl", "kapzaag", "welletjes", "bissen", "catgut", "oxymoron", "heerschaar", "ureter",
            "kijkbuis", "dryade", "grofweg", "laudanum", "excitatie", "revolte", "heugel", "geroerd",
            "hierbij", "glazig", "pussen", "liquide", "aquarium", "formol", "kwelder", "zwager",
            "vuldop", "halfaap", "hansop", "windvaan", "bewogen", "vulstuk", "efemeer", "decisief",
            "omslag", "prairie", "schuit", "weivlies", "ontzeggen", "schijn", "sousafoon"
        };

        public static string Random()
        {
            var random = new Random();
            var index = random.Next(0, words.Length);

            return words[index];
        }
    }
}
