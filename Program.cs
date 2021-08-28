using System;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Birthdays
{
  public class Person
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
  }
  class Program
  {

    static string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Birthday");
    static string dataFilePath = Path.Combine(directoryPath, "data.json");
    static List<Person> personList = new List<Person>();
    static void Main(string[] args)
    {
      FileInit(); // doit intégrer le parsing du JSON
      AcquireData();
      AddPerson();
      /*
      1. Initialiser le fichier json                  -- OK
      2. Charger le fichier et le déserialiser
      3. Chercher des gens
      4. Ajouter des gens                             -- en cours
      6. Modifier des gens
      7. Supprimer des gens
      8. Serialiser le fichier avec les données MaJ.
      */


    }

    /// <summary>Checks if data.json file exists in AppData/Roaming/Birthdays.
    /// If not, creates it. If it fails to do so, throw an FileLoadException()</summary>
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

    static void AddPerson()
    {
      Person person = new Person();

      Console.WriteLine("Entrez le prénom de la personne :");
      person.FirstName = Console.ReadLine();
      Console.WriteLine("Entrez le nom de la personne :");
      person.LastName = Console.ReadLine();
      Console.WriteLine($"Entrez la date de naissance de {person.FirstName} {person.LastName} au format JJ/MM/AAAA :");
      string inputDate = Console.ReadLine();
      string[] splitInput = inputDate.Split('/');
      int day = int.Parse(splitInput[0]);
      int month = int.Parse(splitInput[1]);
      int year = int.Parse(splitInput[2]);

      person.DateOfBirth = new DateTime(year, month, day);
      personList.Add(person);
      Console.WriteLine($"{person.FirstName} {person.LastName}, a vu le jour le {day}/{month}/{year}.");
      Console.WriteLine("Les données ont bien été enregistrées.");
      string jsonString = JsonSerializer.Serialize(personList, new JsonSerializerOptions() { WriteIndented = true });
      Console.WriteLine(jsonString);
      File.WriteAllText(dataFilePath, jsonString);
    }

    static void AcquireData()
    {
      FileInfo fileInfo = new FileInfo(dataFilePath);
      if (fileInfo.Length > 0)
      {
        string jsonString = File.ReadAllText(dataFilePath);
        personList = JsonSerializer.Deserialize<List<Person>>(jsonString);
      }
    }
  }
}
