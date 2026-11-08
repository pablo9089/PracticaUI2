namespace PracticaUI2;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void OnLoginClicked(object sender, EventArgs e)
	{
		if (sender is not Button button)
			return;

		var email = EmailEntry.Text?.Trim();
		var password = PasswordEntry.Text;

		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			await DisplayAlertAsync("Datos incompletos", "Introduce tu correo y contraseña para continuar.", "Aceptar");
			return;
		}

		button.IsEnabled = false;

		try
		{
			await ApiService.LoginAsync(email, password);
			await DisplayAlertAsync("Inicio de sesión", "Has iniciado sesión correctamente.", "Aceptar");
		}
		catch (ApiException ex)
		{
			await DisplayAlertAsync("No se pudo iniciar sesión", ex.Message, "Aceptar");
		}
		catch (HttpRequestException)
		{
			await DisplayAlertAsync("API no disponible", "No se pudo conectar con PracticaApi2. Comprueba que está ejecutándose en el puerto 5080.", "Aceptar");
		}
		catch (TaskCanceledException)
		{
			await DisplayAlertAsync("Tiempo de espera agotado", "La API tardó demasiado en responder.", "Aceptar");
		}
		finally
		{
			button.IsEnabled = true;
		}
	}
}