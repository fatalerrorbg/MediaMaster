using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMaster.ConsoleApp
{
    public class MainMenu
    {
        public List<string> MenuItems { get; private set; }

        public MainMenu()
        {
            this.MenuItems = new List<string>();
        }

        public MainMenu(params string[] menuItems)
            : this()
        {
            foreach (var item in menuItems)
            {
                this.MenuItems.Add(item);
            }
        }

        public void Render(bool clear = true)
        {
            Console.Clear();

            for (int i = 0; i < this.MenuItems.Count; i++)
            {
                string itemText = string.Format("{0}. {1}", i, this.MenuItems[i]);
                Console.WriteLine(itemText);
            }
        }

        public string ReadItem()
        {
            int selectedItem = 0;
            string text = string.Empty;
            while (!int.TryParse(text = Console.ReadLine(), out selectedItem)) { }

            return this.MenuItems[selectedItem];
        }
    }
}
