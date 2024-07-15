using System.Timers;
using Timer = System.Timers.Timer;
namespace PersonalTask;

public partial class MainPage : ContentPage
{
	private readonly LocalDbService _localDbService;
	private int _editDutyId;

	private DateTime timeStarted;
	private bool running;


	public MainPage(LocalDbService localDbService)
	{
		InitializeComponent();
		_localDbService = localDbService;
		ResetAllData();
		Task.Run(async () => listView.ItemsSource = await _localDbService.GetDuties());
	}

	private async void ResetAllData()
	{
		var duties = await _localDbService.GetDuties(); // Fetch current duties

		foreach (var duty in duties)
		{
			if (duty.CompletedDate.HasValue && duty.CompletedDate.Value.Date != DateTime.Today)
			{
				// Reset properties
				duty.Sets = 0;
				duty.IsDone = false;
				duty.CompletedDate = null;

				// Update the duty in the database
				await _localDbService.Update(duty);
			}
		}

		// Refresh the listView
		listView.ItemsSource = duties;
	}


	private void OnStopButtonClicked(object sender, EventArgs e)
	{
		running = false;

		var button = sender as Button;
		if (button == null) return;
		var duty = button.CommandParameter as Duty;
		if(duty == null) return;


	}

	private async void OnResetButtonClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button == null) return;
		var duty = button.CommandParameter as Duty;
		if(duty == null) return;

		if(button.Text == "Reset")
		{
			await _localDbService.Update(new Duty{
				Id = duty.Id,
				Name = duty.Name,
				IsDone = false,
				// Sets = duty.Sets,
				Duration = TimeSpan.Zero,
			});
		}	
		listView.ItemsSource = await _localDbService.GetDuties();
	}


	private async void OnPlayButtonClicked(object sender, EventArgs e)
	{
		var button = sender as Button;
		if (button == null) return;
		var duty = button.CommandParameter as Duty;
		if(duty == null) return;

		if(button.Text == "Play"){

			button.Text = "Stop";
			timeStarted = DateTime.Now;
			running = true;
			Thread thread = new Thread(timer);
			thread.Start();

		}else{

			button.Text = "Play";
			running = false;
			
			await _localDbService.Update(new Duty{
				Id = duty.Id,
				Name = duty.Name,
				IsDone = true,
				Sets = duty.Sets + 1,
				Duration = TimeSpan.Parse(timerlabel.Text),
				CompletedDate = DateTime.Now,
			});

			timerlabel.Text = String.Empty;
			listView.ItemsSource = await _localDbService.GetDuties();
		}
	}

	private void timer(){
		if(!running){
			return;
		}
		DateTime time = DateTime.Now;
		TimeSpan timePassed = time - timeStarted;
		Dispatcher.Dispatch(new Action(()=>{
			timerlabel.Text = string.Format("{0:mm\\:ss}", timePassed);
		}));
		Thread.Sleep(400);
		timer();
	}



	private void saveAcionButtonClickTest(object sender, EventArgs e)
	{
		DisplayAlert("Save Action", "Save Action Clicked", "Ok");
	}

    private async void saveAcionButtonClick(object sender, EventArgs e)
	{
		// Console.WriteLine("Save Button Clicked");
		if(string.IsNullOrWhiteSpace(nameEntryField.Text))
		{
			DisplayAlert("Error", "Please fill all fields", "Ok");
			return;
		}

		// int setsCount = int.Parse(setsEntryField.Text);
		// SetRecord[] setRecords = new SetRecord[setsCount];
		// for(int i = 0; i < setsCount; i++){
		// 	setRecords[i] = new SetRecord{
		// 		count = 0,
		// 		CompletedDate = null,
		// 	};
		// }


		if(_editDutyId == 0){
			await _localDbService.Create(new Duty{
				Name = nameEntryField.Text,
				IsDone = false,
				Sets = 0,
				// SetRecords = setRecords,
			});
		}else{
			//Edit Customer
			await _localDbService.Update(new Duty{
				Id = _editDutyId,
				Name = nameEntryField.Text,
				IsDone = false,
				Sets = 0,
				// SetRecords = setRecords,
			});
		}

		_editDutyId = 0;
		nameEntryField.Text = string.Empty;
		// setsEntryField.Text = string.Empty;
		listView.ItemsSource = await _localDbService.GetDuties();
	}

	private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
	{
		var duty = (Duty)e.Item;
		var action = await DisplayActionSheet("Actions", "Cancel", null, "Edit", "Delete");

		// int setsCount = duty.Sets;
		// SetRecord[] setRecords = new SetRecord[setsCount];
		// for(int i = 0; i < setsCount; i++){
		// 	setRecords[i] = new SetRecord{
		// 		count = 0,
		// 		CompletedDate = null,
		// 	};
		// }

		switch(action)
		{
			case "Edit":
					_editDutyId = duty.Id;
					nameEntryField.Text = duty.Name;
					// setsEntryField.Text = duty.Sets.ToString();
				break;
			case "Delete":
					await _localDbService.Delete(duty);
					listView.ItemsSource = await _localDbService.GetDuties();
				break;
		}
	}

}

