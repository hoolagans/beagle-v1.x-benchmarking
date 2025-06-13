namespace Supermodel.Mobile.Runtime.Common.XForms.App;

public class FormsApplication<TApp> : FormsApplication where TApp : SupermodelXamarinFormsApp, new()
{
    public static TApp RunningApp => (TApp)_runningApp;
}
public class FormsApplication
{
    public static void SetRunningApp(SupermodelXamarinFormsApp runningApp) { _runningApp = runningApp; }
    public static SupermodelXamarinFormsApp GetRunningApp() { return _runningApp; }
        
    // ReSharper disable once InconsistentNaming
    protected static SupermodelXamarinFormsApp _runningApp;
}