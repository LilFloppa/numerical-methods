using GravityGradiometry;
using Microsoft.AspNetCore.Components;
using UI.Models;

namespace UI.Pages
{
    public partial class Index: ComponentBase
    {
        public Axis xAxis = new();
        public Axis zAxis = new();

        public Problem problem = null;
        public Grid grid = null;

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
        }

        public void OnCalculate()
        {

        }

        public void OnResetGrid()
        {
            grid = null;
        }
    }
}
