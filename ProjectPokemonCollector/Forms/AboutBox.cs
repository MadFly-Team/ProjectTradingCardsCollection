using ProjectMTGCollector.Models;
using ProjectMTGCollector.Services;

namespace ProjectMTGCollector.Forms;

public partial class AboutBox : Form
{
    private readonly PokemonCardService _pokemonCardService;
    private readonly ImageCacheService _imageCacheService;

    public AboutBox()
    {
        InitializeComponent();
        _pokemonCardService = new PokemonCardService();
        _imageCacheService = new ImageCacheService();
        _ = LoadRandomCard();
    }

    private async void BtnNewCard_Click( object? sender, EventArgs e )
    {
        await LoadRandomCard();
    }

    private async Task LoadRandomCard()
    {
        btnNewCard.Enabled = false;
        lblRandomCard.Text = "Loading random card...";
        picRandomCard.Image = null;

        try
        {
            var randomCard = await _pokemonCardService.GetRandomCardAsync();
            if (randomCard == null)
            {
                lblRandomCard.Text = "Could not load a random Pokémon card";
                return;
            }

            lblRandomCard.Text = $"{randomCard.Name}\n{randomCard.SetName} ({randomCard.Set.ToUpper()})";
            var imageUrl = randomCard.ImageUris?.Normal;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var image = await _imageCacheService.GetImageAsync(imageUrl);
                if (image != null)
                    picRandomCard.Image = image;
            }
        }
        catch ( Exception ex )
        {
            lblRandomCard.Text = $"Error: {ex.Message}";
        }
        finally
        {
            btnNewCard.Enabled = true;
        }
    }

    private void lblDescription_Click( object sender, EventArgs e )
    {

    }
}
