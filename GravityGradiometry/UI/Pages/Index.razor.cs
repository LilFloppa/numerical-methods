using GravityGradiometry;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using UI.Models;

namespace UI.Pages
{
    public partial class Index: ComponentBase
    {
        public Axis xAxis = new();
        public Axis zAxis = new();

        public Problem problem = null;
        public Grid grid = null;
        public Grid initialGrid = null;

        public ColorScale colorScale = new ColorScale();

        public void OnBuildGrid()
        {
            if (grid != null)
            {
                Logger.LogWarning("Сетка уже построена. Если вы хотите построить новую, то нажмите сначала на кнопку [Сбросить сетку]");
                return;
            }

            if (!xAxis.Validate())
            {
                Logger.LogError("Данные для оси X не валидны");
                return;
            }
            
            if (!zAxis.Validate())
            {
                Logger.LogError("Данные для оси Z не валидны");
                return;
            }
            
            ReceiversInfo receiversInfo = new();             
            receiversInfo.BeginX = -2000;
            receiversInfo.EndX = 6000;
            receiversInfo.Count = 800;

            double[] x = xAxis.Build();
            double[] z = zAxis.Build();
            int k = xAxis.CellCount * zAxis.CellCount;

            double[] properties = new double[k];
            grid = new(x, z, properties, receiversInfo.BuildReceivers());
            initialGrid = grid;

            UpdateColorScale();

            Logger.LogInformation("Сетка построена");
        }

        public async Task OnCalculate()
        {
            if (grid == null)
            {
                Logger.LogError("Невозможно посчитать задачу, сетка не построена");
                return;
            }

            double[] gamma = new double[grid.Properties.Length];
            problem = new Problem(grid, new Regularization { Alpha = 0.0, Gamma = new double[grid.Properties.Length] });
            double[] soultion = await Task.Run(() => problem.Solve());
            grid.Properties = soultion;

            UpdateColorScale();
        }

        public void OnResetGrid()
        {
            grid = null;
            initialGrid = null;
        }

        public void OnGridCellChange(ChangeEventArgs args, int index)
        {
            if (grid == null)
            {
                Logger.LogError("Невозможно изменить значение ячейки, так как сетка не построена");
                return;
            }

            string? stringValue = args?.Value?.ToString();

            if (stringValue == null)
            {
                // TODO: Add logging
                return;
            }

            stringValue = stringValue.Replace(",", ".");
            if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                grid.Properties[index] = result;
                UpdateColorScale();;
            }
            else
            {
                // TODO: Add logging
                StateHasChanged();
            }
        }

        private void UpdateColorScale()
        {
            if (grid != null)
                colorScale.SetValues(grid.Properties);
        }
    }
}
