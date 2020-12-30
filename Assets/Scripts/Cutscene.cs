namespace TheBunniesOfVegetaria
{
    [System.Serializable]
    public class Cutscene
    {
        public string sceneToLoad;
        public string music;
        public string[] text;
        public string[] spriteFileNames;

        // Optional fields
        public bool startAtBoss;
        public Globals.Area nextArea;
        public string nextCutscene;
    }
}