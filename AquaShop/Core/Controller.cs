using AquaShop.Core.Contracts;
using AquaShop.Models.Aquariums;
using AquaShop.Models.Aquariums.Contracts;
using AquaShop.Models.Decorations;
using AquaShop.Models.Decorations.Contracts;
using AquaShop.Models.Fish;
using AquaShop.Models.Fish.Contracts;
using AquaShop.Repositories;
using AquaShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AquaShop.Core
{
    public class Controller : IController
    {
        private DecorationRepository decorations;
        private IList<IAquarium> aquariums;

        public Controller()
        {
            this.decorations = new DecorationRepository();
            this.aquariums = new List<IAquarium>();
        }
        public string AddAquarium(string aquariumType, string aquariumName)
        {
            if (aquariumType == "FreshwaterAquarium" || aquariumType == "SaltwaterAquarium")
            {
                IAquarium aquarium = null;
                if (aquariumType == "FreshwaterAquarium")
                {
                    aquarium = new FreshwaterAquarium(aquariumName);
                }
                else
                {
                    aquarium = new SaltwaterAquarium(aquariumName);
                }

                this.aquariums.Add(aquarium);
                return string.Format(OutputMessages.SuccessfullyAdded, aquariumType);
            }
            else
            {
                throw new InvalidOperationException(ExceptionMessages.InvalidAquariumType);
            }
        }

        public string AddDecoration(string decorationType)
        {
            if (decorationType == "Ornament" || decorationType == "Plant")
            {
                IDecoration decoration = null;
                if (decorationType == "Ornament")
                {
                    decoration = new Ornament();
                }
                else
                {
                    decoration = new Plant();
                }

                this.decorations.Add(decoration);
                return string.Format(OutputMessages.SuccessfullyAdded, decorationType);
            }
            else
            {
                throw new InvalidOperationException(ExceptionMessages.InvalidDecorationType);
            }
        }

        public string AddFish(string aquariumName, string fishType, string fishName, string fishSpecies, decimal price)
        {
            if (fishType == "FreshwaterFish" || fishType == "SaltwaterFish")
            {
                var aquarium = this.aquariums.First(x => x.Name == aquariumName);
                var aquariumType = aquarium.GetType().Name;

                if (fishType == "FreshwaterFish" && aquariumType == "FreshwaterAquarium")
                {
                    var fish = new FreshwaterFish(fishName, fishSpecies, price);
                    aquarium.AddFish(fish);
                    return string.Format(OutputMessages.FishAdded, fishType, aquariumName);
                }
                else if (fishType == "SaltwaterFish" && aquariumType == "SaltwaterAquarium")
                {
                    var fish = new SaltwaterFish(fishName, fishSpecies, price);
                    aquarium.AddFish(fish);
                    return string.Format(OutputMessages.FishAdded, fishType, aquariumName);
                }
                else
                {
                    return OutputMessages.UnsuitableWater;
                }
            }
            else
            {
                throw new InvalidOperationException(ExceptionMessages.InvalidFishType);
            }
        }

        public string CalculateValue(string aquariumName)
        {
            var aquarium = this.aquariums.First(x => x.Name == aquariumName);

            var sumFishPrices = aquarium.Fish.Sum(x => x.Price);
            var sumDecorationPrices = aquarium.Decorations.Sum(x => x.Price);

            var valueAquarium = sumFishPrices + sumDecorationPrices;

            return string.Format(OutputMessages.AquariumValue, aquariumName, valueAquarium);
        }

        public string FeedFish(string aquariumName)
        {
            var aquarium = this.aquariums.First(x => x.Name == aquariumName);

            aquarium.Feed();

            return string.Format(OutputMessages.FishFed, aquarium.Fish.Count());
        }

        public string InsertDecoration(string aquariumName, string decorationType)
        {
            var decoration = this.decorations.FindByType(decorationType);

            if (decoration == null)
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.InexistentDecoration, decorationType));
                //throw new InvalidOperationException(ExceptionMessages.InexistentDecoration, decorationType);
            }
            else
            {
                this.aquariums.First(x => x.Name == aquariumName).AddDecoration(decoration);
                
                this.decorations.Remove(decoration);

                return string.Format(OutputMessages.DecorationAdded, decorationType, aquariumName);
            }
        }

        public string Report()
        {
            var str = new StringBuilder();

            foreach (var aquarium in this.aquariums)
            {
                str.AppendLine(aquarium.GetInfo());
            }

            return str.ToString().TrimEnd();
        }
    }
}
