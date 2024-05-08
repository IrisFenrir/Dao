using UnityEngine;

namespace Dao.WordSystem
{
    public class Word
    {
        public string id { get; private set; }
        public string standardTranslation { get; private set; }
        public bool getAnswer { get; private set; }
        public Sprite image { get; private set; }

        public string customTranslation { get; set; }

        public Word(string id, string standardTranslation, string imageName)
        {
            this.id = id;
            this.standardTranslation = standardTranslation;
            image = Resources.Load<Sprite>("Ciphertext/" + imageName);
        }

        public void GetAnswer()
        {
            getAnswer = true;
        }

        public string GetTranslation()
        {
            return getAnswer ? standardTranslation : customTranslation;
        }
    }
}
