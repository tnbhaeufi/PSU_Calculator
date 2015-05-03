﻿using PSU_Calculator.Dateizugriffe;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PSU_Calculator.DataWorker
{
  public class PSUCalculatorSettings
  {
    private static PSUCalculatorSettings instance = null;
    public static string Version = "Version";
    public static string CPU = "CPUs";
    public static string GPU = "GPUs";
    public static string PowerSupply = "Netzteile";
    public static string MySystem = "MySystem";
    public static string ChoosenComponents = "Components";
    public static string Einstellungen = "Einstellungen";
    public static string DataPath = "PSU_Calculator_Data";
    public static string SearchEngineString = "Suchmaschine";
    public static string DefaultSearchEngine = "de";
    private static string BoolValues = "Booleans";

    private string searchEngineString = "";

    public string PathToDataOrdner
    {
      get;
      set;
    }

    public double GPUVersion
    {
      get
      {
        return GetAsDouble(GetSetting("Version." + GPU).getAttribut("Version"));
      }
      set
      {
        GetSetting("Version." + GPU).addAttribut("Version", value.ToString());
      }
    }

    public double CPUVersion
    {
      get
      {
        return GetAsDouble(GetSetting("Version." + CPU).getAttribut("Version"));
      }
      set
      {
        GetSetting("Version." + CPU).addAttribut("Version", value.ToString());
      }
    }

    public double PSUVersion
    {
      get
      {
        return GetAsDouble(GetSetting("Version." + PowerSupply).getAttribut("Version"));
      }
      set
      {
        GetSetting("Version." + PowerSupply).addAttribut("Version", value.ToString());
      }
    }

    Dictionary<string, Element> ElementDict = new Dictionary<string, Element>();
    private bool hasChanged = false;
    private Element Settings = null;

    public StringJoiner Daten;

    public static PSUCalculatorSettings Get()
    {
      if (instance == null)
      {
        instance = new PSUCalculatorSettings();
        instance.Load();
      }
      return instance;
    }

    private static string AppDataPath
    {
      get
      {
        return Environment.ExpandEnvironmentVariables("%appdata%");
      }
    }

    private static string LocalPath
    {
      get
      {
        return Application.StartupPath;
      }
    }

    private PSUCalculatorSettings()
    {
      PathToDataOrdner = AppDataPath;
      if (Directory.Exists(DirectoryPath))
      {
        return;
      }
      PathToDataOrdner = LocalPath;
    }

    private void Load()
    {
      Settings = StorageMapper.GetXML(GetXmlFilePath(Einstellungen));
      Element version;
      if (Settings.getAllEntries().Count == 0)
      {
        Settings.Name = "Settings";
        version = new Element("Version");
        Settings.addElement(version);
        Daten = new StringJoiner(StorageMapper.ReadFromFilesystem(GetFilePath(Einstellungen)), true);

        //Zuerst die Elemente Hinzufügen, erst danach abfragen auf die einzelnen Versionen machen
        version.addElement(Element.New(GPU).addAttribut("Version", Daten.GetValueForKeyAsDouble(GPU).ToString()));
        version.addElement(Element.New(CPU).addAttribut("Version", Daten.GetValueForKeyAsDouble(CPU).ToString()));
        version.addElement(Element.New(PowerSupply).addAttribut("Version", Daten.GetValueForKeyAsDouble(PowerSupply).ToString()));

        string search = Daten.GetValueForKey(SearchEngineString);
        if (string.IsNullOrEmpty(search))
        {
          search = "DE";
        }
        Settings.addElement(new Element(SearchEngineString, search));
        Settings.addElement(new Element(BoolValues));
        hasChanged = true;
        SaveSettings();
      }
      version = Settings.getElementByName("Version");
      //ElementDict abfüllen.
      ElementDict.Add(GPU, version.getElementByName(GPU));
      ElementDict.Add(CPU, version.getElementByName(CPU));
      ElementDict.Add(PowerSupply, version.getElementByName(PowerSupply));
      ElementDict.Add(SearchEngineString, Settings.getElementByName(SearchEngineString));
      if (Settings.getElementByName(BoolValues) != null)
      {
        ElementDict.Add(BoolValues, Settings.getElementByName(BoolValues));
      }
      SetSearchEngine(Settings.getElementByName(SearchEngineString).Text);
    }

    public void ChangeDataPathToLocal()
    {
      string tmpPath=LocalPath + Path.DirectorySeparatorChar + DataPath;
      if (Directory.Exists(tmpPath))
      {
        return;
      }
      Directory.CreateDirectory(tmpPath);
      DirectoryCopy(DirectoryPath, tmpPath, true);
      string oldPath = DirectoryPath;
      PathToDataOrdner = LocalPath;
      Directory.Delete(oldPath, true);
    }

    public void ChangeDataPathToAppdata()
    {
      string tmpPath = AppDataPath + Path.DirectorySeparatorChar + DataPath;
      if (Directory.Exists(tmpPath))
      {
        return;
      }
      Directory.CreateDirectory(tmpPath);
      DirectoryCopy(DirectoryPath, tmpPath, true);
      string oldPath=DirectoryPath;
      PathToDataOrdner = AppDataPath;
      Directory.Delete(oldPath, true);
    }

    private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
      DirectoryInfo dir = new DirectoryInfo(sourceDirName);
      DirectoryInfo[] dirs = dir.GetDirectories();

      // If the source directory does not exist, throw an exception.
      if (!dir.Exists)
      {
        throw new DirectoryNotFoundException(
            "Source directory does not exist or could not be found: "
            + sourceDirName);
      }

      // If the destination directory does not exist, create it.
      if (!Directory.Exists(destDirName))
      {
        Directory.CreateDirectory(destDirName);
      }


      // Get the file contents of the directory to copy.
      FileInfo[] files = dir.GetFiles();

      foreach (FileInfo file in files)
      {
        // Create the path to the new copy of the file.
        string temppath = Path.Combine(destDirName, file.Name);

        // Copy the file.
        file.CopyTo(temppath, false);
      }

      // If copySubDirs is true, copy the subdirectories.
      if (copySubDirs)
      {

        foreach (DirectoryInfo subdir in dirs)
        {
          // Create the subdirectory.
          string temppath = Path.Combine(destDirName, subdir.Name);

          // Copy the subdirectories.
          DirectoryCopy(subdir.FullName, temppath, copySubDirs);
        }
      }
    }

    private double GetAsDouble(string data)
    {
      double output;
      if (double.TryParse(data, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out output))
      {
        return output;
      }
      return 0.0d;
    }

    public Element GetSetting(string settingName)
    {
      return Settings.getElementByPfadOnCreate(settingName);
    }

    public void SetSearchEngine(string engine)
    {
      OverrideSetting(PSUCalculatorSettings.SearchEngineString, engine);
      string oldEngine = SearchEngine;
      //Daten.Put(SearchEngineString, engine);
      SearchEngine = engine;
      if (oldEngine != null && !oldEngine.Equals(SearchEngine))
      {
        hasChanged = true;
      }
    }

    public void OverrideSetting(string key, string value)
    {
      hasChanged = true;
      Element ele;
      if (!ElementDict.TryGetValue(key, out ele))
      {
        ele = new Element(key);
        Settings.addElement(ele);
        ElementDict.Add(key, ele);
      }
      ele.Text = value;
      //Daten.Put(key, value);
    }

    public void OverrideSetting(string key, Element newElement)
    {
      hasChanged = true;
      Element oldElement;
      if (ElementDict.TryGetValue(key, out oldElement))
      {
        ElementDict.Remove(key);
        Settings.removeElement(oldElement);
      }
      Settings.addElement(newElement);
      ElementDict.Add(key, newElement);
      SaveSettings();
    }

    public void SaveSettings()
    {
      if (hasChanged)
      {
        StorageMapper.WriteToFilesystem(GetXmlFilePath(Einstellungen), Settings.getXML());
        hasChanged = false;
      }
    }

    public bool HasChanged
    {
      get
      {
        return hasChanged;
      }
    }

    public bool ShowPriceComparer
    {
      get;
      set;
    }
    //public bool ShowPriceComparer
    //{
    //  get
    //  {
    //    return GetBoolValue("ShowPriceComparer");
    //  }
    //  set
    //  {
    //    SetBoolValue("ShowPriceComparer", value);
    //  }
    //}

    public bool ConnectorsHaveToFit
    {
      get
      {
        return GetBoolValue("ConnectorsHaveToFit");
      }
      set
      {
        SetBoolValue("ConnectorsHaveToFit", value);
      }
    }

    /// <summary>
    /// Setzt einen Boolschen Wert in das Setting Element
    /// </summary>
    /// <param name="key">Name des Boolschen wertes</param>
    /// <param name="value">Wert</param>
    private void SetBoolValue(string key, bool value)
    {
      Element ele;
      if (!ElementDict.TryGetValue(BoolValues, out ele))
      {
        ele = new Element(BoolValues);
      }
      ele.addAttribut(key, value.ToString());
      OverrideSetting(BoolValues, ele);
    }

    /// <summary>
    /// gibt einen Boolschen wert asu dem Settings Element zurück.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private bool GetBoolValue(string key)
    {
      Element ele;
      if (!ElementDict.TryGetValue(BoolValues, out ele))
      {
        return true;
      }
      Boolean output = false;
      if (Boolean.TryParse(ele.getAttribut(key), out output))
      {
        return output;
      }
      return true;
    }

    public static string GetFilePath(string filename)
    {
      return PSUCalculatorSettings.Get().GetPath(filename, "data");
    }

    public static string GetXmlFilePath(string filename)
    {
      return PSUCalculatorSettings.Get().GetPath(filename, "xml");
    }

    private string GetPath(string filename, string ending)
    {
      return string.Format("{0}{1}.{2}", AbsolutDataPath, filename, ending);
    }

    public string SearchEngine
    {
      get
      {
        return searchEngineString;
      }
      private set
      {
        searchEngineString = value;
      }
    }

    private string AbsolutDataPath
    {
      get
      {
        return string.Format("{0}{2}{1}{2}", PathToDataOrdner, PSUCalculatorSettings.DataPath, Path.DirectorySeparatorChar);
      }
    }

    //Pfade im Dateisystem
    public string CPUPath
    {
      get
      {
        return string.Format("{0}{3}{1}{3}{2}.txt", PathToDataOrdner, PSUCalculatorSettings.DataPath, PSUCalculatorSettings.CPU, Path.DirectorySeparatorChar);
      }
    }

    public string GPUPath
    {
      get
      {
        return string.Format("{0}{3}{1}{3}{2}.txt", PathToDataOrdner, PSUCalculatorSettings.DataPath, PSUCalculatorSettings.GPU, Path.DirectorySeparatorChar);
      }
    }

    public string DirectoryPath
    {
      get
      {
        return string.Format("{0}{2}{1}", PathToDataOrdner, PSUCalculatorSettings.DataPath, Path.DirectorySeparatorChar);
      }
    }

    public override string ToString()
    {
      return Daten.ToString();
    }
  }
}