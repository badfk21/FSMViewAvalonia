
using Path = Avalonia.Controls.Shapes.Path;

namespace FSMViewAvalonia2;

public class UINode
{
    public Grid grid;
    public RectangleGeometry rectGeom;
    public Border border;
    public Path rectPath;
    public TextBlock label;
    public SolidColorBrush stroke;
    public FsmStateData stateData;
    public FsmNodeData nodeData;
    public FsmTransition[] transitions;
    public string name;
    private bool selected;

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;

            border.BorderBrush = selected
                ? Brushes.LightBlue
                : stroke;

            border.BorderThickness = selected
                ? new Thickness(3)
                : new Thickness(2);

            Rect transform = nodeData.transform;

            //add border and fix offset
            Transform = selected
            ? new Rect(transform.X - 1, transform.Y - 1, transform.Width + 4, transform.Height + 5)
            : new Rect(transform.X, transform.Y, transform.Width + 2, transform.Height + 3);
        }
    }

    public Rect Transform
    {
        get => new(grid.GetValue(Canvas.LeftProperty),
                            grid.GetValue(Canvas.TopProperty),
                            rectGeom.Rect.Width,
                            rectGeom.Rect.Height);
        set
        {
            _ = grid.SetValue(Canvas.LeftProperty, value.X);
            _ = grid.SetValue(Canvas.TopProperty, value.Y);
            //todo
            //rect.Width = value.Width;
            //rect.Height = value.Height;
        }
    }

    public UINode(FsmStateData stateData, FsmNodeData nodeData) :
                    this(stateData, nodeData, new SolidColorBrush(stateData?.isStartState ?? false ? Colors.Gold : Colors.Black))
    { }

    public UINode(FsmStateData stateData, FsmNodeData nodeData, SolidColorBrush stroke)
    {
        this.stateData = stateData;
        this.nodeData = nodeData;
        transitions = nodeData.transitions;
        name = nodeData.name;

        this.stroke = stroke;

        bool isGlobal = nodeData.isGlobal;

        Rect transform = nodeData.transform;

        grid = new Grid();
        _ = grid.SetValue(Canvas.LeftProperty, transform.X);
        _ = grid.SetValue(Canvas.TopProperty, transform.Y);

        FontFamily font = new("Segoe UI Bold");

        StackPanel stack = new();

        label = new TextBlock
        {
            Foreground = Brushes.White,
            Text = name,
            FontFamily = font,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            TextAlignment = TextAlignment.Center,
            Background = new SolidColorBrush(nodeData.stateColor),
            MaxWidth = transform.Width,
            MinWidth = transform.Width
        };

        if (isGlobal)
        {
            label.Background = new SolidColorBrush(Color.FromRgb(0x20, 0x20, 0x20));
        }

        stack.Children.Add(label);

        if (!isGlobal)
        {
            foreach (FsmTransition transition in transitions)
            {
                stack.Children.Add(new TextBlock
                {
                    Background = new SolidColorBrush(nodeData.transitionColor),
                    Foreground = Brushes.DimGray,
                    Text = transition.fsmEvent.name,
                    FontFamily = font,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Center,
                    MaxWidth = transform.Width,
                    MinWidth = transform.Width
                });
            }

            var list = stack.Children.OfType<TextBlock>().ToList();
            for (int index = 0; index < list.Count; index++)
            {
                TextBlock i = list[index];
                Grid.SetRow(i, index);

                //stops lowercase descenders in the state titles
                //from getting cut-off
                i.MaxHeight = index == 0
                    ? (i.MinHeight = (transform.Height / list.Count) + 1.4)
                    : (i.MinHeight = (transform.Height - 1.4) / list.Count);
            }
        }

        border = new Border()
        {
            Child = stack,
            BorderBrush = stroke,
            BorderThickness = new Thickness(2),
            Padding = new Thickness(0)
        };

        grid.Children.Add(border);
    }
}
