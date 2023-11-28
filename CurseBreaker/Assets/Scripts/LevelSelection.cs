using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UI;
using System.Drawing;

public class LevelSelection : MonoBehaviour
{
    private int currentCharacter;

    // Start is called before the first frame update
    private void Start()
    {
        CheckCharacterProgression();
    }

    private void CheckCharacterProgression()
    {
        currentCharacter = PlayerPrefs.GetInt("currentCharacter");
        GameObject[] charAvatars = GameObject.FindGameObjectsWithTag("CharSelectionAvatar");

        foreach (GameObject charAvatar in charAvatars)
        {
            string name = charAvatar.name;
            char lastCharacter = name[name.Length - 1];
            int i = (int)char.GetNumericValue(lastCharacter);

            Image avatarFrame = null;
            Image avatarImage = null;
            Transform avatarButton = null;

            Sprite[] spritesheet = null;
            string spriteName = "";
            Sprite curedSprite = null;

            foreach(Transform child in charAvatar.transform)
            {
                if(child.name.Contains("PictureExample"))
                {
                    Image image = child.GetComponent<Image>();
                    avatarFrame = image;
                }else if(child.name.Contains("Character"))
                {
                    Image image = child.GetComponent<Image>();
                    avatarImage = image;
                }else if(child.name.Contains("SelectLevelButton"))
                {
                    avatarButton = child;
                }
            }

            switch (i)
            {
                case 0:
                    //bunny sprites
                    spritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                    spriteName = "sideCharacters-0_6";
                    break;
                case 1:
                    //dog sprites
                    spritesheet = Resources.LoadAll<Sprite>("sideCharacters-1");
                    spriteName = "sideCharacters-1_6";
                    break;
                case 2:
                    //owl sprites
                    spritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                    spriteName = "sideCharacters-0_5";
                    break;
                case 3:
                    //cat sprites
                    spritesheet = Resources.LoadAll<Sprite>("sideCharacters-1");
                    spriteName = "sideCharacters-1_7";
                    break;
                case 4:
                    //ox sprites
                    spritesheet = Resources.LoadAll<Sprite>("sideCharacters-0");
                    spriteName = "sideCharacters-0_7";
                    break;
            }

            foreach (Sprite sprite in spritesheet)
            {
                if (sprite.name == spriteName)
                {
                    curedSprite = sprite;
                }
            }

            if(i < currentCharacter)
            {
                avatarImage.sprite = curedSprite;
            }else if(i > currentCharacter)
            {
                avatarImage.color = new Color32(0, 0, 0, 255);
                avatarFrame.color = new Color32(103, 103, 103, 255);
                avatarButton.gameObject.SetActive(false);
            }
        }
    }
}
