namespace StairsLight.ColorControllers
{
    interface IColorController
    {
        void Refresh();
        float Brightness { get; set; }
    }
}