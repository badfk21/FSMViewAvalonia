using FSMViewAvalonia2.Context;

namespace FSMViewAvalonia2;
public class FsmDataInstanceUI(FsmDataInstance fsm, GameContext ctx)
{
    public FsmDataInstance fsm = fsm;
    public int tabIndex;
    public List<UINode> nodes;
    public Controls canvasControls;
    public Matrix matrix;
    public List<FsmStateData> states = fsm.states.Select(x => new FsmStateData(x)).ToList();
    public GameContext context = ctx;

    //To prevent memory leak because Avalonia's bugs
    public void Detach()
    {
        fsm = null;
        nodes = null;
        canvasControls = null;
        states = null;
        context = null;
    }
}
