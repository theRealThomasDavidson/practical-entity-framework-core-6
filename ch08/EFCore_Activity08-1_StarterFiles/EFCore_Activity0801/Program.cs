using System;
using EFCore_DBLibrary;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace EFCore_Activity0801
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private static DbContextOptionsBuilder<AdventureWorksContext> _optionsBuilder;


        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<AdventureWorksContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AdventureWorks"));
        }


        private static void ListPeopleThenOrderAndTake()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var people = db.People.ToList().OrderByDescending(x => x.LastName);
                foreach (var person in people.Take(10))
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName}");
                }
            }
        }


        private static void QueryPeopleOrderedToListAndTake()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var query = db.People.OrderByDescending(x => x.LastName);
                var result = query.Take(10).AsNoTracking();
                foreach (var person in result)
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName}");
                }
            }
        }


        private static void FilteredPeople(string filter)
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var searchTerm = filter.ToLower();
                var query = db.People.Where(x =>
                       x.LastName.ToLower().Contains(searchTerm)
                    || x.FirstName.ToLower().Contains(searchTerm)
                    || x.PersonType.ToLower().Equals(searchTerm)
                ).AsNoTracking();
                foreach (var person in query)
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName}, {person.PersonType}");
                }
            }
        }


        private static void FilteredAndPagedResult(string filter, int pageNumber, int pageSize)
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var searchTerm = filter.ToLower();
                var query = db.People.Where(x => 
                       x.LastName  .ToLower().Contains(searchTerm)
                    || x.FirstName .ToLower().Contains(searchTerm)
                    || x.PersonType.ToLower().Equals(searchTerm)
                ).AsNoTracking().OrderBy(x => x.LastName)
                .Skip(pageNumber * pageSize)
                .Take(pageSize);
                foreach (var person in query)
                {
                    Console.WriteLine($"{person.FirstName} {person.LastName}, { person.PersonType}");
                }
            }
        }


        static void Main(string[] args)
        {
            BuildOptions();

            Console.WriteLine("List People Then Order and Take");
            ListPeopleThenOrderAndTake();
            Console.WriteLine("Query People, order, then list and take");
            QueryPeopleOrderedToListAndTake();
            Console.WriteLine("Please Enter the partial First or Last Name, or the Person Type to search for:");
            var result = Console.ReadLine();
            FilteredPeople(result);
            int pageSize = 10;
            for (int pageNumber = 0; pageNumber < 10; pageNumber++)
            {
                Console.WriteLine($"Page {pageNumber + 1}");
                FilteredAndPagedResult(result, pageNumber, pageSize);
            }

        }

    }
}


