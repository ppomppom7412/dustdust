using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CollectionScripts;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.Utils;
using NaughtyAttributes;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.U2D.Animation;

//종족 리스트
public enum PresetList { None, Human, Elf, DarkElf, Demon, Goblin, Orc, Lizard, FireLizard, ZombieA, ZombieB, Skeleton, Vampire, Demigod, Merman, Furry, Werewolf, Max };

public class CharacterSpriteBulider : MonoBehaviour
{
    [Foldout("Datas")]
    public string Preset = "Human";
    [Foldout("Datas")]
    public string Hair;
    [Foldout("Datas")]
    public string Armor;
    [Foldout("Datas")]
    public string Helmet;
    [Foldout("Datas")]
    public string Weapon;
    [Foldout("Datas")]
    public string Firearm;
    [Foldout("Datas")]
    public string Shield;
    [Foldout("Datas")]
    public string Cape;
    [Foldout("Datas")]
    public string Back;
    [Foldout("Datas")]
    public string Mask;
    [Foldout("Datas")]
    public string Horns;

    public Texture2D Texture { get; private set; }
    private Dictionary<string, Sprite> _sprites;

    [Header("Sprite Components")]
    public SpriteCollection spCollection;
    public SpriteLibrary spLibrary;

    /// <summary>
    /// 조각들을 모아 하나의 텍스처로 결합시킨다.
    /// </summary>
    /// <param name="changed"></param>
    /// <param name="forceMerge"></param>
    [NaughtyAttributes.Button]
    public void Rebuild(string changed = null, bool forceMerge = false)
    {
        //var width = SpriteCollection.Layers[0].Textures[0].width;
        //var height = SpriteCollection.Layers[0].Textures[0].height;
        var width = 576;
        var height = 928;
        var dict = spCollection.Layers.ToDictionary(i => i.Name, i => i);
        var layers = new Dictionary<string, Color32[]>();

        if (Preset.Contains("Lizard")) Hair = Helmet = Mask = "";

        if (Back != "") layers.Add("Back", dict["Back"].GetPixels(Back, null, changed));
        if (Shield != "") layers.Add("Shield", dict["Shield"].GetPixels(Shield, null, changed));

        if (Preset != "")
        {
            layers.Add("Body", dict["Body"].GetPixels(Preset, null, changed));

            if (Firearm == "")
            {
                var arms = dict["Arms"].GetPixels(Preset, null, changed == "Body" ? "Arms" : changed).ToArray();

                layers.Add("Arms", arms);
            }
        }

        if (Preset != "") layers.Add("Head", dict["Head"].GetPixels(Preset, null, changed));
        if (Preset != "" && (Helmet == "" || Helmet.Contains("[ShowEars]"))) layers.Add("Ears", dict["Ears"].GetPixels(Preset, null, changed));

        if (Armor != "")
        {
            layers.Add("Armor", dict["Armor"].GetPixels(Armor, null, changed));

            if (Firearm == "")
            {
                layers.Add("Bracers", dict["Bracers"].GetPixels(Armor, null, changed == "Armor" ? "Bracers" : changed));
            }
        }

        if (Preset != "") layers.Add("Eyes", dict["Eyes"].GetPixels(Preset, null, changed));
        if (Hair != "") layers.Add("Hair", dict["Hair"].GetPixels(Hair, Helmet == "" ? null : layers["Head"], changed));
        if (Cape != "") layers.Add("Cape", dict["Cape"].GetPixels(Cape, null, changed));
        if (Helmet != "") layers.Add("Helmet", dict["Helmet"].GetPixels(Helmet, null, changed));
        if (Weapon != "") layers.Add("Weapon", dict["Weapon"].GetPixels(Weapon, null, changed));

        if (Mask != "") layers.Add("Mask", dict["Mask"].GetPixels(Mask, null, changed));
        if (Horns != "" && Helmet == "") layers.Add("Horns", dict["Horns"].GetPixels(Horns, null, changed));

        var order = spCollection.Layers.Select(i => i.Name).ToList();

        layers = layers.Where(i => i.Value != null).OrderBy(i => order.IndexOf(i.Key)).ToDictionary(i => i.Key, i => i.Value);

        if (Texture == null) Texture = new Texture2D(width, height) { filterMode = UnityEngine.FilterMode.Point };

        if (Shield != "")
        {
            var shield = layers["Shield"];
            var last = layers.Last(i => i.Key != "Weapon");
            var copy = last.Value.ToArray();

            for (var i = 2 * 64 * width; i < 3 * 64 * width; i++)
            {
                if (shield[i].a > 0) copy[i] = shield[i];
            }

            layers[last.Key] = copy;
        }

        if (Firearm != "")
        {
            foreach (var layerName in new[] { "Head", "Ears", "Eyes", "Mask", "Hair", "Helmet" })
            {
                if (!layers.ContainsKey(layerName)) continue;

                var copy = layers[layerName].ToArray();

                for (var y = 11 * 64 - 1; y >= 10 * 64 - 1; y--)
                {
                    for (var x = 0; x < width; x++)
                    {
                        copy[x + y * width] = copy[x + (y - 1) * width];
                    }
                }

                layers[layerName] = copy;
            }
        }

        Texture = TextureHelper.MergeLayers(Texture, layers.Values.ToArray());
        Texture.SetPixels(0, Texture.height - 32, 32, 32, new Color[32 * 32]);

        if (Cape != "") CapeOverlay(layers["Cape"]);

        if (_sprites == null)
        {
            var clipNames = new List<string> { "Idle", "Ready", "Run", "Crawl", "Climb", "Jump", "Push", "Jab", "Slash", "Shot", "Fire", "Block", "Death", "Roll" };

            clipNames.Reverse();

            _sprites = new Dictionary<string, Sprite>();

            for (var i = 0; i < clipNames.Count; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    var key = clipNames[i] + "_" + j;

                    _sprites.Add(key, Sprite.Create(Texture, new Rect(j * 64, i * 64, 64, 64), new Vector2(0.5f, 0.125f), 16, 0, SpriteMeshType.FullRect));
                }
            }
        }

        var spriteLibraryAsset = ScriptableObject.CreateInstance<UnityEngine.U2D.Animation.SpriteLibraryAsset>();

        foreach (var sprite in _sprites)
        {
            var split = sprite.Key.Split('_');

            spriteLibraryAsset.AddCategoryLabel(sprite.Value, split[0], split[1]);
        }

        spLibrary.spriteLibraryAsset = spriteLibraryAsset;

    }

    /// <summary>
    /// 등 뒤에 있는 망토를 오버레이 시킨다.
    /// </summary>
    /// <param name="cape"></param>
    private void CapeOverlay(Color32[] cape)
    {
        if (Cape == "") return;

        var pixels = Texture.GetPixels32();
        var width = Texture.width;
        var height = Texture.height;

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                //if (x >= 0 && x < 2 * 64 && y >= 9 * 64 && y < 10 * 64 // "Climb_0", "Climb_1"
                //    || x >= 64 && x < 64 + 2 * 64 && y >= 6 * 64 && y < 7 * 64 // "Jab_1", "Jab_2"
                //    || x >= 128 && x < 128 + 2 * 64 && y >= 5 * 64 && y < 6 * 64 // "Slash_2", "Slash_3"
                //    || x >= 0 && x < 4 * 64 && y >= 4 * 64 && y < 5 * 64) // "Shot_0", "Shot_1", "Shot_2", "Shot_3"
                if (x >= 0 && x < 2 * 64 && y >= 9 * 64 && y < 10 * 64 // "Climb_0", "Climb_1"
                    || x >= 64 && x < 64 + 2 * 64 && y >= 6 * 64 && y < 7 * 64 // "Jab_1", "Jab_2"
                    || x >= 128 && x < 128 + 2 * 64 && y >= 5 * 64 && y < 6 * 64 // "Slash_2", "Slash_3"
                    || x >= 0 && x < 4 * 64 && y >= 4 * 64 && y < 5 * 64) // "Shot_0", "Shot_1", "Shot_2", "Shot_3"
                {
                    var i = x + y * width;

                    if (cape[i].a > 0) pixels[i] = cape[i];
                }
            }
        }

        Texture.SetPixels32(pixels);
        Texture.Apply();
    }
    
    /// <summary>
    /// 텍스트 데이터를 분리시켜 텍스쳐화 시킨다.
    /// </summary>
    /// <param name="data"></param>
    public void RebuildToString(string data)
    {
        string[] sprits = data.Split(',');

        for (int i = 0; i < sprits.Length; ++i)
        {
            switch (i)
            {
                case 0:
                    Preset = sprits[i];
                    break;

                case 1:
                    Hair = sprits[i];
                    break;

                case 2:
                    Armor = sprits[i];
                    break;

                case 3:
                    Helmet = sprits[i];
                    break;

                case 4:
                    Weapon = sprits[i];
                    break;

                case 5:
                    Firearm = sprits[i];
                    break;

                case 6:
                    Shield = sprits[i];
                    break;

                case 7:
                    Cape = sprits[i];
                    break;

                case 8:
                    Back = sprits[i];
                    break;

                case 9:
                    Mask = sprits[i];
                    break;

                case 10:
                    Horns = sprits[i];
                    break;

                default:
                    break;
            }
        }

        Rebuild();
    }

}