using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timeline : MonoBehaviour
{
    public void UpdateTimeline(List<string> types)
    {
        RawImage[] images = GetComponentsInChildren<RawImage>();
        int index = 0;
        foreach (var placing in types)
        {
            string type = "boss placing";

            switch(placing)
            {
                case "A":
                    type = "adven placing a";
                break;

                case "B":
                    type = "adven placing b";
                break;

                case "C":
                    type = "adven placing c";
                break;

                case "D":
                    type = "adven placing d";
                break;

                case "E":
                    type = "adven placing e";
                break;
            }

            images[index++].texture = Resources.Load<Texture>(type);
        }
    }
}
