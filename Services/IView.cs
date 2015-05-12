namespace Services
{
    public interface IView
    {
        object DataContext { get; set; }

        void Show();
    }
}