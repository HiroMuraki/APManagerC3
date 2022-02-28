namespace APManagerC3 {
    public interface IViewModel<TModel, TViewModel> {
        TViewModel LoadFromModel(TModel model);
        TModel ConvertToModel();
    }
}
