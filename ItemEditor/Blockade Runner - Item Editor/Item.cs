﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Deployment;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace Blockade_Runner___Item_Editor
{

    [System.Diagnostics.DebuggerDisplay("Index:{Index} Name:{Name} UniqueID:{UniqueID}")]
    [XmlRoot("Item")]
    public class BR_ItemData
    {
        public int Index = -1;
        public bool Modified = false;

        public string Name { get; set; }
        [XmlElement("Unique_Id")]
        public string UniqueID { get; set; }
        public string Icon { get; set; }
        public string Model { get; set; }
        [XmlElement("Model_LOD")]
        public string ModelLOD { get; set; }
        [XmlElement("Model_GUI")]
        public string ModelGUI { get; set; }
        [XmlElement("Model_Texture")]
        public string Model_TextureDiffuse { get; set; }
        public string Model_TextureNormals { get; set; }
        public string ModelGUI_TextureDiffuse { get; set; }
        public string ModelGUI_TextureNormals { get; set; }
        public string Image { get; set; }

        [XmlElement("Scale_BaseMin")]
        public float Size_BaseMin { get; set; }
        [XmlElement("Scale_BaseMax")]
        public float Size_BaseMax { get; set; }
        public float Mass { get; set; }

        public int StackMax { get; set; }
        public int ResourceFragment { get; set; }

        public bool Model_CounterCullingMode { get; set; }
        [XmlElement("Two_Handed")]
        public bool TwoHanded { get; set; }

        [XmlElement("Size2d")]
        [TypeConverter(typeof(Vector2Converter))]
        public Vector2 Size_2D { get; set; }

        [XmlElement("Scale_Min")]
        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 Size_Min { get; set; }

        [XmlElement("Scale_Max")]
        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 Size_Max { get; set; }

        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 Model_Position { get; set; }


        public Variables Variables { get; set; }
        //[XmlElement("Strings")]
        //public List<StringString> Variables_String { get; set; }
        ////public System.Collections.Generic.Dictionary<string, string> Variables_String { get; set; }

        //[XmlElement("Variables")]
        //public List<StringFloat> Variables_Float { get; set; }
        public List<StringFloat> Resource { get; set; }

        public List<StringInt> CraftedWith { get; set; }

        [XmlElement("Primary_function")]
        public Methods PrimaryMethod { get; set; }
        [XmlElement("Secondary_function")]
        public Methods SecondaryMethod { get; set; }

        [XmlElement("Inventory_function")]
        public InventoryMethods InventoryMethod { get; set; }

        public Type Category { get; set; }

        public BR_ItemData()
        {

            Index = 0;
            Initialize();

        }

        public BR_ItemData(int index)
        {

            Index = index;
            Initialize();

        }

        private void Initialize()
        {
            Size_BaseMin = 1;
            Size_BaseMax = 1;
            Mass = 0;
            StackMax = 1;
            ResourceFragment = 0;
            Size_2D = new Vector2(1, 1);
            Size_Min = new Vector3(1, 1, 1);
            Size_Max = new Vector3(1, 1, 1);
            Model_Position = new Vector3(-0.5f, 0.35f, -0.7f);
            //Variables_String = new Dictionary<string, string>();
            //Variables_String = new List<StringString>();
            //Variables_Float = new List<StringFloat>();
            Resource = new List<StringFloat>();
            CraftedWith = new List<StringInt>();
            PrimaryMethod = Methods.Melee;
            SecondaryMethod = Methods.Melee;
            InventoryMethod = InventoryMethods.None;
        }

        public static void Save(BR_ItemData item, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BR_ItemData));
            TextWriter textWriter = new StreamWriter(filename);
            serializer.Serialize(textWriter, item);
            textWriter.Close();
        }

        public static BR_ItemData Load(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(BR_ItemData));
            TextReader textReader = new StreamReader(filename);
            BR_ItemData item;
            item = (BR_ItemData)deserializer.Deserialize(textReader);
            textReader.Close();
            return item;
        }

        public static BR_ItemData LoadItemFromPath(string path, int index)
        {
            string dir = path + "Item.txt";
            if (!System.IO.File.Exists(dir))
                return null;

            BR_ItemData item = new BR_ItemData(index);

            item = Load(dir);

            return item;
            //return OldManualXMLLoad(path, dir, item);
        }

        public static BR_ItemData OldManualXMLLoad(string path, string dir, BR_ItemData item)
        {

            XmlDocument xd = new XmlDocument();
            xd.Load(dir);
            XmlNodeList dataNodes = xd.SelectNodes("Item");

            bool recieved_gui_texture = false;

            foreach (XmlNode n in dataNodes[0].ChildNodes)
            {
                switch (n.Name.ToLower())
                {
                    case "name":
                        item.Name = n.InnerText;
                        break;
                    case "icon":
                    case "icon_texture":
                        if (System.IO.File.Exists(path + n.InnerText))
                            item.Icon = n.InnerText;
                        break;
                    case "texture":
                    case "model_texture":
                        item.Model_TextureDiffuse = n.InnerText;
                        item.Model_TextureNormals = n.InnerText;
                        break;
                    case "gui_texture":
                        item.ModelGUI_TextureDiffuse = n.InnerText;
                        item.ModelGUI_TextureNormals = n.InnerText;
                        recieved_gui_texture = true;
                        break;
                    case "model":
                    case "mesh":
                        item.Model = n.InnerText;
                        break;
                    case "meshlod":
                    case "mesh_lod":
                    case "modellod":
                    case "model_lod":
                        item.ModelLOD = n.InnerText;
                        break;
                    case "meshgui":
                    case "mesh_gui":
                    case "modelgui":
                    case "model_gui":
                        item.ModelGUI = n.InnerText;
                        break;
                    case "identifier":
                    case "uniqueid":
                    case "unique_id":
                        item.UniqueID = n.InnerText.Length > 8 ? n.InnerText.Substring(0, 8) : n.InnerText;
                        break;
                    case "two_handed":
                    case "twohanded":
                    case "two-handed":
                        item.TwoHanded = Convert.ToBoolean(n.InnerText);
                        break;
                    case "mass":
                        item.Mass = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "scale":
                        item.Size_Min = new Vector3(float.Parse(n.Attributes.GetNamedItem("X").InnerText, System.Globalization.CultureInfo.InvariantCulture), float.Parse(n.Attributes.GetNamedItem("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture), float.Parse(n.Attributes.GetNamedItem("Z").InnerText, System.Globalization.CultureInfo.InvariantCulture));
                        item.Size_Max = item.Size_Min;
                        item.Size_BaseMin = Math.Min(item.Size_Min.X, Math.Min(item.Size_Min.Y, item.Size_Min.Z));
                        item.Size_BaseMax = Math.Min(item.Size_Max.X, Math.Min(item.Size_Max.Y, item.Size_Max.Z));
                        break;
                    case "scale_min":
                        item.Size_Min = new Vector3(float.Parse(n.Attributes.GetNamedItem("X").InnerText, System.Globalization.CultureInfo.InvariantCulture), float.Parse(n.Attributes.GetNamedItem("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture), float.Parse(n.Attributes.GetNamedItem("Z").InnerText, System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    case "scale_max":
                        item.Size_Max = new Vector3(float.Parse(n.Attributes.GetNamedItem("X").InnerText, System.Globalization.CultureInfo.InvariantCulture), float.Parse(n.Attributes.GetNamedItem("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture), float.Parse(n.Attributes.GetNamedItem("Z").InnerText, System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    case "scale_basemin":
                        item.Size_BaseMin = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "scale_basemax":
                        item.Size_BaseMax = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "stack_max":
                        item.StackMax = int.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "category":
                        {
                            string s = n.InnerText.ToLower();
                            switch (s)
                            {
                                case "accessory":
                                    item.Category |= Type.Accessory;
                                    break;
                                case "belt":
                                    item.Category |= Type.Belt;
                                    break;
                                case "body":
                                    item.Category |= Type.Body;
                                    break;
                                case "equipment":
                                    item.Category |= Type.Equipment;
                                    break;
                                case "feet":
                                    item.Category |= Type.Feet;
                                    break;
                                case "hand":
                                    item.Category |= Type.Hand;
                                    break;
                                case "head":
                                    item.Category |= Type.Head;
                                    break;
                                case "shoulder":
                                    item.Category |= Type.Shoulder;
                                    break;
                            }
                            //item.MyType = (Type)int.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    case "image":
                        {
                            item.Image = n.InnerText;
                            break;
                        }
                    case "model_position":
                        {
                            for (int i = 0; i < n.Attributes.Count; i++)
                            {
                                if (n.Attributes[i].Name.ToLower() == "x")
                                    item.Model_Position.X = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                                if (n.Attributes[i].Name.ToLower() == "y")
                                    item.Model_Position.Y = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                                if (n.Attributes[i].Name.ToLower() == "z")
                                    item.Model_Position.Z = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            break;
                        }
                    case "model_positionx":
                        {
                            item.Model_Position.X = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    case "model_positiony":
                        {
                            item.Model_Position.Y = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    case "model_positionz":
                        {
                            item.Model_Position.Z = float.Parse(n.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        }
                    case "craftedwith":
                        {
                            StringInt tmp = new StringInt("", 0);
                            for (int i = 0; i < n.Attributes.Count; i++)
                            {
                                if (n.Attributes[i].Name.ToLower() == "item")
                                    tmp.String = n.Attributes[i].InnerText.ToLower();
                                else if (n.Attributes[i].Name.ToLower() == "quantity")
                                    tmp.Int = int.Parse(n.Attributes[i].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            item.CraftedWith.Add(tmp);
                            break;
                        }
                    case "resource":
                        {
                            item.ResourceFragment = int.Parse(n.Attributes[0].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                            foreach (XmlNode cn in n.ChildNodes)
                            {
                                StringFloat tmp = new StringFloat("", 0);
                                for (int i = 0; i < cn.Attributes.Count; i++)
                                {
                                    if (cn.Attributes[i].Name.ToLower() == "item")
                                        tmp.String = cn.Attributes[i].InnerText.ToLower();
                                    else if (cn.Attributes[i].Name.ToLower() == "chance")
                                        tmp.Float = float.Parse(cn.Attributes[i].InnerText, System.Globalization.CultureInfo.InvariantCulture);
                                }
                                item.Resource.Add(tmp);
                            }
                            break;
                        }
                    case "size2":
                    case "size2d":
                        item.Size_2D = new Vector2(float.Parse(n.Attributes.GetNamedItem("X").InnerText, System.Globalization.CultureInfo.InvariantCulture), float.Parse(n.Attributes.GetNamedItem("Y").InnerText, System.Globalization.CultureInfo.InvariantCulture));
                        break;

                    #region FUNCTIONS
                    case "function":
                    case "function1":
                    case "primaryfunction":
                    case "primary_function":
                        for (int i = 0; i < (int)Methods.None; i++)
                            if (n.InnerText == ((Methods)i).ToString())
                            {
                                item.PrimaryMethod = (Methods)i;
                                break;
                            }
                        break;
                    case "function2":
                    case "secondaryfunction":
                    case "secondary_function":
                        for (int i = 0; i < (int)Methods.None; i++)
                            if (n.InnerText == ((Methods)i).ToString())
                            {
                                item.SecondaryMethod = (Methods)i;
                                break;
                            }
                        break;
                    case "functioni":
                    case "function_i":
                    case "inventoryfunction":
                    case "inventory_function":
                        for (int i = 0; i < (int)InventoryMethods.None; i++)
                            if (n.InnerText == ((InventoryMethods)i).ToString())
                            {
                                item.InventoryMethod = (InventoryMethods)i;
                                break;
                            }
                        break;
                    #endregion

                    #region VARIABLES
                    case "variables":
                        foreach (XmlNode n2 in n.ChildNodes)
                        {
                            //item.Variables_Float.Add(new StringFloat(n2.Name, float.Parse(n2.InnerText, System.Globalization.CultureInfo.InvariantCulture)));
                        }
                        break;
                    case "strings":
                        foreach (XmlNode n2 in n.ChildNodes)
                        {
                            //item.Variables_String.Add(new StringString(n2.Name.Length > 8 ? n2.Name.Substring(0, 8) : n2.Name, n2.InnerText));
                        }
                        break;
                    #endregion
                }
            }

            if (!recieved_gui_texture)
            {
                item.ModelGUI_TextureDiffuse = item.Model_TextureDiffuse;
                item.ModelGUI_TextureNormals = item.Model_TextureNormals;
            }

            return item;
        }

        public enum Type : ushort
        {
            Head = 1,
            Body = 2,
            Belt = 4,
            Hand = 8,
            Feet = 16,
            Shoulder = 32,
            Accessory = 64,
            Equipment = 128
        }

        public enum Methods : ushort
        {
            Melee,
            Midas,
            CreateBlock,
            CreateBlockFromInventory,
            None
        }

        public enum InventoryMethods : ushort
        {
            None
        }
    }
}
