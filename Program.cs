using System;
using System.Text.Json;
using System.IO;
using System.Diagnostics;

namespace Birthdays
{
  public class Personne
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
  }
  class Program
  {

    static string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Birthday");
    static string dataFilePath = Path.Combine(directoryPath, "data.json");
    static void Main(string[] args)
    {
      FileInit();


    }

    static void FileInit()
    {
      if (File.Exists(dataFilePath))
      {
        Debug.WriteLine("data.json existe.");
      }
      else
      {
        Directory.CreateDirectory(directoryPath);
        File.Create(dataFilePath);
        Debug.WriteLine("data.json absent, création du fichier.");
        if (!File.Exists(dataFilePath))
          throw new FileLoadException();
      }
    }


  }
}
