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

        public void Backup()
        {
            states.Push(originator.Save());
        }

        public void Undo()
        {
            if (states.Count == 0)
                return;

            State state = states.Pop();
            originator.Restore(state);
        }
    }

    public partial class Index: ComponentBase
    {
        public History history;

        public Problem problem = new();
        public Axis xAxis = new();
        public Axis zAxis = new();
        public ReceiversInfo receiversInfo = new();

        public int k = 0;
        public double[] properties;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            history = new History(this);
        }

        public void OnBuildGridClick()
        {
            history.Backup();

            k = (xAxis.CellCount + 1) * (zAxis.CellCount + 1);
            properties = new double[k];

            double[] x = xAxis.Build();
            double[] z = zAxis.Build();

            Point[] receivers = receiversInfo.BuildReceivers();

            //TODO: check if x or z is not null
            //TODO: check if receivers is not null

            problem.X = x;
            problem.Z = z;
            problem.Receivers = receivers;
        }

        public void OnSetProperties()
        {

        }

        public void OnCalculateClick()
        {

        }

        public void OnUndoClick()
        {
            history.Undo();
        }

        public State Save()
        {
            State state = new State();
            if (properties != null)
            {
                state.properties = new double[properties.Length];
                Array.Copy(properties, state.properties, properties.Length);
            }
            state.xAxis = (Axis)xAxis.Clone();
            state.zAxis = (Axis)zAxis.Clone();
            state.receiversInfo = (ReceiversInfo)receiversInfo.Clone();

            return state;
        }

        public void Restore(State state)
        {
            xAxis = state.xAxis;
            zAxis = state.zAxis;
            receiversInfo = state.receiversInfo;
            properties = state.properties;
        }
    }
}
