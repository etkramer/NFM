using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace NFM;

public abstract class ToolPanel : UserControl
{
	public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<ToolPanel, string>(nameof(Title), "Tool Panel");

	public string Title
	{
		get { return GetValue(TitleProperty); }
		set { SetValue(TitleProperty, value); }
	}
}

public abstract class ReactiveToolPanel<TViewModel> : ToolPanel, IViewFor<TViewModel> where TViewModel : class
{
	public static readonly StyledProperty<TViewModel> ViewModelProperty = AvaloniaProperty.Register<ReactiveUserControl<TViewModel>, TViewModel>(nameof(ViewModel));

        public ReactiveToolPanel()
        {
            // This WhenActivated block calls ViewModel's WhenActivated
            // block if the ViewModel implements IActivatableViewModel.
            this.WhenActivated(disposables => { });
            this.GetObservable(ViewModelProperty).Subscribe(OnViewModelChanged);
        }

        public TViewModel ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            ViewModel = DataContext as TViewModel;
        }

        private void OnViewModelChanged(object value)
        {
            if (value is null)
            {
                ClearValue(DataContextProperty);
            }
            else if (DataContext != value)
            {
                DataContext = value;
            }
        }
}
