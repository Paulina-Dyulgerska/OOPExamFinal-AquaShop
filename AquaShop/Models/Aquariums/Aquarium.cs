using AquaShop.Models.Aquariums.Contracts;
using AquaShop.Models.Decorations.Contracts;
using AquaShop.Models.Fish.Contracts;
using AquaShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AquaShop.Models.Aquariums
{
    public abstract class Aquarium : IAquarium
    {
        private int capacity;
        private string name;
        private List<IDecoration> decorations;
        private List<IFish> fish;

        public Aquarium(string name, int capacity)
        {
            this.Name = name;
            this.Capacity = capacity;
            this.fish = new List<IFish>();
            this.decorations = new List<IDecoration>();
        }

        public string Name
        {
            get => this.name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(ExceptionMessages.InvalidAquariumName);
                }
                this.name = value;
            }
        }

        public int Capacity
        {
            get => this.capacity;
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentException(ExceptionMessages.InvalidAquariumCapacity);
                }
                this.capacity = value;
            }
        }
        public int Comfort => this.decorations.Sum(x => x.Comfort);

        public ICollection<IDecoration> Decorations => this.decorations.AsReadOnly();

        public ICollection<IFish> Fish => this.fish.AsReadOnly();

        public void AddDecoration(IDecoration decoration)
        {
            this.decorations.Add(decoration);
        }

        public void AddFish(IFish fish)
        {
            if (this.Capacity > this.fish.Count())
            {
                this.fish.Add(fish);
            }
            else
            {
                throw new InvalidOperationException(OutputMessages.NotEnoughCapacity);
            }
        }

        public void Feed()
        {
            foreach (var riba in this.fish)
            {
                riba.Eat();
            }
        }

        public string GetInfo()
        {
            var str = new StringBuilder();

            str.AppendLine($"{this.Name} ({this.GetType().Name}):");

            if (this.fish.Count == 0)
            {
                str.AppendLine($"Fish: none");
            }
            else
            {
                str.AppendLine($"Fish: {string.Join(", ", this.fish.Select(x => x.Name))}");
            }

            str.AppendLine($"Decorations: {this.decorations.Count}");
            str.AppendLine($"Comfort: { this.Comfort}");

            return str.ToString().TrimEnd();
        }

        public bool RemoveFish(IFish fish)
        {
            return this.fish.Remove(fish);
        }
    }
}
