using System.Collections.Generic;
using UnityEngine;

public sealed class LocalizationStrings
{
    private static LocalizationStrings _instance;

    private LocalizationStrings()
    {
        Values = new Dictionary<string, string>();

        switch (Application.systemLanguage)
        {
            default:
            case SystemLanguage.French:

                Values.Add("Loading", "Chargement...");
                Values.Add("GameShortDesc", "Cell est sur le point de détruire la terre, l'unique moyen pour l'arrêter et de le vaincre dans ce dernier affrontement. Le destin de l'humanité repose sur toi !");
                Values.Add("Play", "Jouer");

                break;

        }
    }

    public Dictionary<string, string> Values { get; set; }

    public static LocalizationStrings Instance
    {
        get { return _instance ?? (_instance = new LocalizationStrings()); }
    }
}