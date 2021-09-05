using System;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Humanizer;

namespace Birthdays
{
  public class Person
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
  }

  public struct Announcement
  {
    public string announcement;
    public int age;
    public int daysBeforeNextBirthday;

  }
  class Program
  {

    static string m_directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Birthday");
    static string m_dataFilePath = Path.Combine(m_directoryPath, "data.json");
    static List<Person> m_personList = new List<Person>();

    static string[] m_MenuEntries =
    {
        "Choisissez une action ou appuyez sur q pour quitter.\n",
        "u:\tchercher une personne",
        "t:\tafficher tous les anniversaires",
        "p:\tafficher les prochains anniversaires",
        "a:\tajouter un anniversaire",
        "m:\tmodifier un anniversaire",
        "s:\tsupprimer un anniversaire"
    };



    static void Main(string[] args)
    {
      FileInit(); // doit intégrer le parsing du JSON
      AcquireData();
      UserMenu();
      /*
      1.  Initialiser le fichier json                  -- OK
      2.  Charger le fichier et le déserialiser        -- OK
      3.  Chercher des gens                            -- OK
      4.  Ajouter des gens                             -- OK
      6.  Modifier des gens                            -- OK
      7.  Supprimer des gens                           -- OK
      8.  Serialiser le fichier avec les données MaJ.  -- OK
      9.  Afficher toutes les personnes                -- OK
      10. Implementer menu principal                   -- OK
      11. Trois prochains anniversaires                -- OK
      12. Les anniversaires s'affichent triés          -- OK
      */


    }

    static void CyanConsoleDisplayer(string message = null, string[] entries = null)
    {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.BackgroundColor = ConsoleColor.Black;

      if (entries != null)
      {
        foreach (string e in entries)
        {
          Console.WriteLine(e);
        }
      }

      if (message != null)
      {
        Console.WriteLine(message);
      }

      Console.ForegroundColor = ConsoleColor.Gray;
      Console.BackgroundColor = ConsoleColor.Black;
    }
    static void UserMenu()
    {
      Console.Clear();
      Console.WriteLine("Birthdays 0.1 -- Antony Merle, tous droits réservés\n");
      while (true)
      {
        CyanConsoleDisplayer(null, m_MenuEntries);
        string menuChoice = Console.ReadLine();

        switch (menuChoice)
        {
          case "u":
            PrintBirthdays(MakeBirthdayAnnouncements(SearchPerson()));
            CyanConsoleDisplayer("Appuyez sur une touche pour continuer.");
            Console.ReadKey();
            Console.Clear();
            break;

          case "t":
            PrintBirthdays(MakeBirthdayAnnouncements(m_personList));
            CyanConsoleDisplayer("Appuyez sur une touche pour continuer.");
            Console.ReadKey();
            Console.Clear();
            break;

          case "p":
            PrintBirthdays(FindNextBirthdays(MakeBirthdayAnnouncements(m_personList)));
            CyanConsoleDisplayer("Appuyez sur une touche pour continuer.");
            Console.ReadKey();
            Console.Clear();
            break;

          case "a":
            AddPerson();
            CyanConsoleDisplayer("Appuyez sur une touche pour continuer.");
            Console.ReadKey();
            Console.Clear();
            break;

          case "m":
            modifyPersonData();
            CyanConsoleDisplayer("Appuyez sur une touche pour continuer.");
            Console.ReadKey();
            Console.Clear();
            break;

          case "s":
            DeletePerson();
            CyanConsoleDisplayer("Appuyez sur une touche pour continuer.");
            Console.ReadKey();
            Console.Clear();
            break;

          case "q":
            return;

          default:
            Console.WriteLine($"{menuChoice} n'est pas un choix disponible.");
            CyanConsoleDisplayer("Appuyez sur une touche pour continuer.");
            Console.ReadKey();
            Console.Clear();
            break;
        }
      }
    }

    /// <summary>Checks if data.json file exists in AppData/Roaming/Birthdays.
    /// If not, creates it. If it fails to do so, throw an FileLoadException()</summary>
    static void FileInit()
    {
      if (File.Exists(m_dataFilePath))
      {
        Debug.WriteLine("data.json existe.");
      }
      else
      {
        Directory.CreateDirectory(m_directoryPath);
        File.Create(m_dataFilePath);
        Debug.WriteLine("data.json absent, création du fichier.");
        if (!File.Exists(m_dataFilePath))
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
      m_personList.Add(person);
      Console.WriteLine($"{person.FirstName} {person.LastName}, a vu le jour le {day}/{month}/{year}.");
      Console.WriteLine("Les données ont bien été enregistrées.");
      string jsonString = JsonSerializer.Serialize(m_personList, new JsonSerializerOptions() { WriteIndented = true });
      Console.WriteLine(jsonString);
      File.WriteAllText(m_dataFilePath, jsonString);
    }

    static void AcquireData()
    {
      FileInfo fileInfo = new FileInfo(m_dataFilePath);
      if (fileInfo.Length > 0)
      {
        string jsonString = File.ReadAllText(m_dataFilePath);
        m_personList = JsonSerializer.Deserialize<List<Person>>(jsonString);
      }
    }

    static List<Person> SearchPerson()
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
        filteredList = m_personList.FindAll(p => p.FirstName.ToLower().Contains(inputSplit[0].ToLower()) || p.LastName.ToLower().Contains(inputSplit[0].ToLower()));
      }
      else if (inputSplit.Length == 2)
      {

        filteredList = m_personList.FindAll(p => (
          p.FirstName.ToLower().Contains(inputSplit[0].ToLower()) && p.LastName.ToLower().Contains(inputSplit[1].ToLower()) ||
          p.FirstName.ToLower().Contains(inputSplit[1].ToLower()) && p.LastName.ToLower().Contains(inputSplit[0].ToLower())
        ));
      }
      else
      {
        foreach (string w in inputSplit)
        {
          filteredList = m_personList.FindAll(p => p.FirstName.ToLower().Contains(w.ToLower()) || p.LastName.ToLower().Contains(w.ToLower()));
        }
      }
      return filteredList;
    }


    /// <summary>Takes a list of person, deduce their age and next birthday, then returns an array of Announcement structs.</summary>
    static Announcement[] MakeBirthdayAnnouncements(List<Person> filteredList)
    {
      List<Announcement> birthdayAnnouncements = new List<Announcement>();

      foreach (Person result in filteredList)
      {
        //prochain anniversaire = age (now - dob) + 1 - now
        DateTime now = DateTime.Now;
        bool isLeapYear = DateTime.IsLeapYear(now.Year);
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
          Announcement a = new Announcement();

          a.age = age;
          a.daysBeforeNextBirthday = offsetInDays % yearInDays;
          birthdayAnnouncements.Add(a);
          a.announcement = String.Format($"{result.FirstName} {result.LastName} fête son {age.Ordinalize()} anniversaire aujourd'hui !");
        }
        else
        {
          Announcement a = new Announcement();
          a.age = age;
          a.daysBeforeNextBirthday = offsetInDays % yearInDays;
          a.announcement = String.Format($"{result.FirstName} {result.LastName} fêtera son {age.Ordinalize()} anniversaire le {nextBirthdayDate.ToString("dddd dd MMMM yyyy")}, dans {"jour".ToQuantity(offsetInDays % (yearInDays))} !");
          birthdayAnnouncements.Add(a);
        }
      }
      return FindNextBirthdays(birthdayAnnouncements.ToArray(), birthdayAnnouncements.Count());
    }

    static void modifyPersonData()
    {
      /*
      1. entrer requete
      2. vérifier requete
      3. trouver objet qui correspond
      4. modifier objet
      */
      List<Person> liste = new List<Person>();
      do
      {
        liste = SearchPerson();
      } while (liste.Count != 1);

      Console.WriteLine($"Voulez-vous modifier la fiche de {liste[0].FirstName} {liste[0].LastName} né(e) le {liste[0].DateOfBirth.ToString("dd/MM/yyyy")} ? (O/N)");
      string choix = Console.ReadLine();
      switch (choix.ToLowerInvariant())
      {
        case "o":

          foreach (Person p in m_personList)
          {
            if (liste[0].FirstName.ToLower() == p.FirstName.ToLower() && liste[0].LastName == p.LastName && liste[0].DateOfBirth == p.DateOfBirth)
            {
              m_personList.Remove(p);
              break;
            }
          }

          Console.WriteLine("Voulez-vous modifier le prénom ? O/N");
          string prenomChoix = Console.ReadLine();
          switch (prenomChoix.ToLowerInvariant())
          {
            case "o":
              Console.WriteLine("Entrez le prénom de la personne :");
              liste[0].FirstName = Console.ReadLine();
              break;

            case "n": break;
            default: break;
          }

          Console.WriteLine("Voulez-vous modifier le nom ? O/N");
          string nomChoix = Console.ReadLine();
          switch (nomChoix.ToLowerInvariant())
          {
            case "o":
              Console.WriteLine("Entrez le prénom de la personne :");
              liste[0].FirstName = Console.ReadLine();
              break;

            case "n": break;
            default: break;
          }

          Console.WriteLine("Voulez-vous modifier la date de naissance ? O/N");
          string DDNChoix = Console.ReadLine();
          switch (DDNChoix.ToLowerInvariant())
          {
            case "o":

              Console.WriteLine($"Entrez la date de naissance de {liste[0].FirstName} {liste[0].LastName} au format JJ/MM/AAAA :");
              string inputDate = Console.ReadLine();
              string[] splitInput = inputDate.Split('/');
              int day = int.Parse(splitInput[0]);
              int month = int.Parse(splitInput[1]);
              int year = int.Parse(splitInput[2]);

              liste[0].DateOfBirth = new DateTime(year, month, day);
              break;

            case "n": break;
            default: break;
          }

          m_personList.Add(liste[0]);

          Console.WriteLine($"{liste[0].FirstName} {liste[0].LastName}, a vu le jour le {liste[0].DateOfBirth.ToString("dd/MM/yyyy")}.");
          Console.WriteLine("Les données ont bien été enregistrées.");
          string jsonString = JsonSerializer.Serialize(m_personList, new JsonSerializerOptions() { WriteIndented = true });
          Console.WriteLine(jsonString);
          File.WriteAllText(m_dataFilePath, jsonString);

          break;

        case "n": break;

        default: return;
      }
    }

    static void DeletePerson()
    {
      List<Person> liste = new List<Person>();
      do
      {
        liste = SearchPerson();
      } while (liste.Count != 1);

      Console.WriteLine($"Voulez-vous supprimer la fiche de {liste[0].FirstName} {liste[0].LastName} né(e) le {liste[0].DateOfBirth.ToString("dd/MM/yyyy")} ? (O/N)");
      string choix = Console.ReadLine();
      switch (choix.ToLowerInvariant())
      {
        case "o":
          foreach (Person p in m_personList)
          {
            if (liste[0].FirstName.ToLower() == p.FirstName.ToLower() && liste[0].LastName == p.LastName && liste[0].DateOfBirth == p.DateOfBirth)
            {
              m_personList.Remove(p);

              Console.WriteLine($"La fiche de {liste[0].FirstName} {liste[0].LastName} est bien supprimée.");

              string jsonString = JsonSerializer.Serialize(m_personList, new JsonSerializerOptions() { WriteIndented = true });
              Console.WriteLine(jsonString);
              File.WriteAllText(m_dataFilePath, jsonString);

              break;
            }
          }
          break;

        default: break;
      }
    }

    static void PrintBirthdays(Announcement[] announcements = null)
    {

      if (announcements == null)
      {
        return;
      }
      foreach (Announcement a in announcements)
      {
        Console.WriteLine($"{a.announcement}");
      }
    }

    /// <summary>Takes an array of Annoucement structs, generated by MakeBirthdayAnnouncements and returns n next birthdays.</summary>
    static Announcement[] FindNextBirthdays(Announcement[] announcements, int n = 3)
    {
      // var announcements = MakeBirthdayAnnouncements(m_personList);
      var filteredAnnouncements = announcements.OrderBy(a => a.daysBeforeNextBirthday).Take(n);

      return filteredAnnouncements.ToArray();
    }
  }
}