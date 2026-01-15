namespace kfutils.UI {

    public interface IUIDataProvider <out T> {
        T RetrieveData();
    }

}