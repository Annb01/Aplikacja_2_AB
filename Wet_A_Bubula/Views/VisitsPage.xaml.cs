using Wet_A_Bubula.ViewModels;
using Wet_A_Bubula.Model;
using System.Diagnostics;

namespace Wet_A_Bubula;

public partial class VisitsPage : ContentPage
{
    private VisitsPageViewModel _viewModel;
    public VisitsPage(UserModel user)
    {
        InitializeComponent();
        _viewModel = new VisitsPageViewModel(user);
        BindingContext = _viewModel;
        
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

      
        datePicker.MinimumDate = DateTime.Today.AddDays(1);
    }
    private async void OnAddButtonClicked(object sender, EventArgs e)
    {
        var selectedDate = datePicker.Date;

       
        var selectedTime = timePicker.Time;
        Debug.WriteLine($"xaml.cs -> {selectedDate}{selectedTime}");

        
        DateOnly dataNowejWizyty = DateOnly.FromDateTime(selectedDate);
        TimeOnly czasNowejWizyty = TimeOnly.FromTimeSpan(selectedTime);
        Debug.WriteLine($"xaml.cs -> {dataNowejWizyty}{czasNowejWizyty}");
       
        _viewModel.DataNowejWizyty = dataNowejWizyty;
        _viewModel.CzasNowejWizyty = czasNowejWizyty;

        await _viewModel.VisitsPage();
    }
    
}