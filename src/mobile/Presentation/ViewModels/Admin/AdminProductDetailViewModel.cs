using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShushineStudio.Mobile.Domain.Entities;
using ShushineStudio.Mobile.Domain.Repositories;

namespace ShushineStudio.Mobile.Presentation.ViewModels.Admin;

[QueryProperty(nameof(ProductoId), "id")]
public partial class AdminProductDetailViewModel : ObservableObject
{
    private readonly IProductoRepository _productoRepository;

    [ObservableProperty]
    private string title = "Ficha de Producto";

    [ObservableProperty]
    private int productoId;

    [ObservableProperty]
    private string codigoProducto = string.Empty;

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string marca = string.Empty;

    [ObservableProperty]
    private string nombreCategoria = "Cuidado Capilar";

    [ObservableProperty]
    private string descripcion = string.Empty;

    [ObservableProperty]
    private decimal precio = 0;

    [ObservableProperty]
    private int stockActual = 0;

    [ObservableProperty]
    private int stockMinimo = 5;

    [ObservableProperty]
    private bool activo = true;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isSaving = false;

    [ObservableProperty]
    private bool hasError = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool EsEdicion => ProductoId > 0;
    public bool EsNuevo => ProductoId == 0;

    public AdminProductDetailViewModel(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    partial void OnProductoIdChanged(int value)
    {
        if (value > 0)
        {
            Title = "Editar Producto";
            _ = CargarProductoAsync(value);
        }
        else
        {
            Title = "Nuevo Producto";
            LimpiarFormulario();
        }
        OnPropertyChanged(nameof(EsEdicion));
        OnPropertyChanged(nameof(EsNuevo));
    }

    private void LimpiarFormulario()
    {
        CodigoProducto = $"PROD-{DateTime.Now:MMddHHmm}";
        Nombre = string.Empty;
        Marca = string.Empty;
        NombreCategoria = "Cuidado Capilar";
        Descripcion = string.Empty;
        Precio = 0;
        StockActual = 10;
        StockMinimo = 5;
        Activo = true;
    }

    [RelayCommand]
    public async Task CargarProductoAsync(int id)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var p = await _productoRepository.GetProductoPorIdAsync(id);
            if (p != null)
            {
                CodigoProducto = p.CodigoProducto;
                Nombre = p.Nombre;
                Marca = p.Marca;
                NombreCategoria = string.IsNullOrEmpty(p.NombreCategoria) ? "General" : p.NombreCategoria;
                Descripcion = p.Descripcion ?? string.Empty;
                Precio = p.Precio;
                StockActual = p.StockActual;
                StockMinimo = p.StockMinimo;
                Activo = p.Activo;
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Error al cargar producto: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            await Shell.Current.DisplayAlert("Validación", "El nombre del producto es obligatorio.", "OK");
            return;
        }

        if (Precio <= 0)
        {
            await Shell.Current.DisplayAlert("Validación", "El precio debe ser mayor a 0.", "OK");
            return;
        }

        try
        {
            IsSaving = true;
            var prod = new Producto
            {
                Id = ProductoId,
                CodigoProducto = string.IsNullOrWhiteSpace(CodigoProducto) ? $"PROD-{DateTime.Now:MMddHHmm}" : CodigoProducto.Trim(),
                Nombre = Nombre.Trim(),
                Marca = string.IsNullOrWhiteSpace(Marca) ? "Genérico" : Marca.Trim(),
                NombreCategoria = string.IsNullOrWhiteSpace(NombreCategoria) ? "General" : NombreCategoria.Trim(),
                Descripcion = Descripcion?.Trim(),
                Precio = Precio,
                StockActual = StockActual,
                StockMinimo = StockMinimo,
                Activo = Activo
            };

            if (EsNuevo)
            {
                await _productoRepository.CrearProductoAsync(prod);
                await Shell.Current.DisplayAlert("Éxito", "Producto registrado en el inventario correctamente.", "OK");
            }
            else
            {
                await _productoRepository.ModificarProductoAsync(ProductoId, prod);
                await Shell.Current.DisplayAlert("Éxito", "Producto actualizado correctamente.", "OK");
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo guardar el producto: " + ex.Message, "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (!EsEdicion) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Eliminar Producto",
            $"¿Estás seguro de desactivar o retirar '{Nombre}' del catálogo?",
            "Sí, eliminar", "Cancelar");

        if (!confirm) return;

        try
        {
            IsSaving = true;
            var ok = await _productoRepository.EliminarProductoAsync(ProductoId);
            if (ok)
            {
                await Shell.Current.DisplayAlert("Eliminado", "El producto fue retirado del inventario.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo eliminar el producto: " + ex.Message, "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task RegresarAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
