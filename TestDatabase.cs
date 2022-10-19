using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class TestDatabase : IDatabase
    {
        ObservableCollection<Entry> entries = new ObservableCollection<Entry>();
        public TestDatabase()
        {
            // adds initial test data
            entries.Add(new Entry("Stressful", "COMPUTER SCIENCE", 2, "10-08-22", 1));
            entries.Add(new Entry("Dynamic Island", "IPHONE 14 PRO", 1, "07-09-22", 2));
            entries.Add(new Entry("Superior programming language", "PYTHON", 1, "01-08-20", 3));
        }

        public void AddEntry(Entry entry)
        {
            entries.Add(entry);
        }

        public bool DeleteEntry(Entry entry)
        {
            entries.Remove(entry);
            return true;
        }

        public bool EditEntry(Entry replacementEntry)
        {
            foreach (Entry entry in entries) // iterate through entries until we find the Entry in question
            {
                if (entry.Id == replacementEntry.Id) // found it
                {
                    entry.Clue = replacementEntry.Clue;
                    entry.Answer = replacementEntry.Answer;
                    entry.Difficulty = replacementEntry.Difficulty;
                    entry.Date = replacementEntry.Date;
                    return true;
                }
            }
            return false;
        }

        public Entry FindEntry(int id)
        {
            foreach (Entry entry in entries)
            {
                if (entry.Id == id)
                {
                    return entry;
                }
            }
            return null;
        }

        public ObservableCollection<Entry> GetEntries()
        {
            return entries;
        }

        public int GetNextId()
        {
            int id = 1;
            foreach (Entry entry in entries)
            {
                if (entry.Id > id)
                {
                    id = entry.Id;
                }
            }
            return id;
        }

        public void SortByAnswer()
        {
            throw new NotImplementedException();
        }

        public void SortByClue()
        {
            throw new NotImplementedException();
        }
    }
}
