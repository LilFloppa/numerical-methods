using GravityGradiometry;
using Microsoft.AspNetCore.Components;
using UI.Models;

namespace UI.Pages
{
    public class State
    {
        public Axis xAxis = new();
        public Axis zAxis = new();
        public ReceiversInfo receiversInfo = new();
        public double[] properties = null;
    }

    public class History
    {
        Stack<State> states = new Stack<State>();

        Index originator;
        public History(Index originator) => this.originator = originator;

        //public void Backup()
        //{
        //    states.Push(originator.Save());
        //}

        //public void Undo()
        //{
        //    if (states.Count == 0)
        //        return;

        //    State state = states.Pop();
        //    originator.Restore(state);
        //}
    }

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

        public void OnUndo()
        {
        }

        //public State Save()
        //{
        //    State state = new State();
        //    if (properties != null)
        //    {
        //        state.properties = new double[properties.Length];
        //        Array.Copy(properties, state.properties, properties.Length);
        //    }
        //    state.xAxis = (Axis)xAxis.Clone();
        //    state.zAxis = (Axis)zAxis.Clone();
        //    state.receiversInfo = (ReceiversInfo)receiversInfo.Clone();

        //    return state;
        //}

        //public void Restore(State state)
        //{
        //    xAxis = state.xAxis;
        //    zAxis = state.zAxis;
        //    receiversInfo = state.receiversInfo;
        //    properties = state.properties;
        //}
    }
}
