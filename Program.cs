using System;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Humanizer;

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
      // AddPerson();
      SearchPerson();
      /*
      1. Initialiser le fichier json                  -- OK
      2. Charger le fichier et le déserialiser        -- OK
      3. Chercher des gens
      4. Ajouter des gens                             -- OK
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

    static void SearchPerson()
    {
      Console.WriteLine("Qui recherchez-vous ?");
      string input = Console.ReadLine();
      string[] inputSplit = input.Split(' ');
      List<Person> filteredList = new List<Person>();

      /* Comportement de la recherche :
      Si la recherche ne contient qu'un mot, on teste OR
      Si la recherche contient plus d'un mot, on teste AND
      */
      if (inputSplit.Length == 1)
      {
        filteredList = personList.FindAll(p => p.FirstName.ToLower().Contains(inputSplit[0].ToLower()) || p.LastName.ToLower().Contains(inputSplit[0].ToLower()));
      }
      else if (inputSplit.Length == 2)
      {

        filteredList = personList.FindAll(p => (
          p.FirstName.ToLower().Contains(inputSplit[0].ToLower()) && p.LastName.ToLower().Contains(inputSplit[1].ToLower()) ||
          p.FirstName.ToLower().Contains(inputSplit[1].ToLower()) && p.LastName.ToLower().Contains(inputSplit[0].ToLower())
        ));
      }
      else
      {
        foreach (string w in inputSplit)
        {
          filteredList = personList.FindAll(p => p.FirstName.ToLower().Contains(w.ToLower()) || p.LastName.ToLower().Contains(w.ToLower()));
        }
      }


      Console.WriteLine("Résultats :");
      foreach (Person result in filteredList)
      {
        //prochain anniversaire = age (now - dob) + 1 - now
        DateTime now = DateTime.Now;
        bool isLeapYear = DateTime.IsLeapYear(now.Year);
        // bug : si l'anniversaire est cette année, le calcul est faux.

        bool isNextBirthdayThisYear = now.Month <= result.DateOfBirth.Month && now.Day <= result.DateOfBirth.Day ? true : false;

        DateTime nextBirthdayDate = isNextBirthdayThisYear ?
        result.DateOfBirth.AddYears(now.Year - result.DateOfBirth.Year) :
        result.DateOfBirth.AddYears((now.Year - result.DateOfBirth.Year) + 1);

        int age = isNextBirthdayThisYear ? now.Year - result.DateOfBirth.Year : (now.Year - result.DateOfBirth.Year) + 1;


        TimeSpan timeOffset = nextBirthdayDate - now;
        int offsetInDays = timeOffset.Days + 1;
        int yearInDays = isLeapYear ? 366 : 365;

        if ((result.DateOfBirth.Month == now.Month) && (result.DateOfBirth.Day == now.Day))
        {
          Console.WriteLine($"{result.FirstName} {result.LastName} fête son {age.Ordinalize()} anniversaire aujourd'hui !");
        }
        else
        {
          Console.WriteLine($"{result.FirstName} {result.LastName} fêtera son {age.Ordinalize()} anniversaire le {nextBirthdayDate.ToString("dddd dd MMMM yyyy")} dans {"jour".ToQuantity(offsetInDays % (yearInDays))} !");
        }
      }
    }
  }
}
