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
        public Regularization regularization = new();
        public Grid currentGrid = null;
        public Grid solutionGrid = null;
        public Grid initialGrid = null;

        public ColorScale colorScale = new ColorScale();

        public double F = 0.0;

        public void OnBuildGrid()
        {
            if (initialGrid != null)
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
            initialGrid = new(x, z, properties, receiversInfo.BuildReceivers());
            currentGrid = initialGrid;

            UpdateColorScale();

            Logger.LogInformation("Сетка построена");
        }

        public async Task OnCalculate()
        {
            if (initialGrid == null)
            {
                Logger.LogError("Невозможно посчитать задачу, сетка не построена");
                return;
            }

            if (!regularization.Validate())
            {
                Logger.LogError("Неверные параметры регуляризации");
                return;
            }    

            problem = new Problem(initialGrid, regularization);
            (double[] solution, double f) = await Task.Run(() => problem.Solve());
            solutionGrid = new(initialGrid.X, initialGrid.Z, solution, initialGrid.Receivers);
            currentGrid = solutionGrid;
            F = f;

            UpdateColorScale();
        }

        public void OnResetGrid()
        {
            currentGrid = null;
            initialGrid = null;
            solutionGrid = null;
        }

        public void OnGridCellChange(ChangeEventArgs args, int index)
        {
            if (currentGrid == null)
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
                currentGrid.Properties[index] = result;
                UpdateColorScale();
            }
            else
            {
                // TODO: Add logging
                StateHasChanged();
            }
        }

        public void OnInputChange(ChangeEventArgs args, string name)
        {
            string? stringValue = args?.Value?.ToString();
            if (stringValue == null)
            {
                // TODO: Add logging
                return;
            }

            stringValue = stringValue.Replace(",", ".");
            if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                if (name == "alpha")
                    regularization.Alpha = result;

                if (name == "gamma")
                    regularization.Gamma = result;
                StateHasChanged();          
            }
        }

        private void UpdateColorScale()
        {
            if (currentGrid != null)
                colorScale.SetValues(currentGrid.Properties);
        }

        private void OnBack()
        {
            if (initialGrid != null)
            {
                currentGrid = initialGrid;
                UpdateColorScale();
            }
            else
                Logger.LogError("Невозможно переключиться на исходную сетку, так как она null");
        }

        private void OnForward()
        {
            if (solutionGrid != null)
            {
                currentGrid = solutionGrid;
                UpdateColorScale();
            }
            else
                Logger.LogError("Невозможно переключиться на сетку решения, так как она null");
        }
    }
}
