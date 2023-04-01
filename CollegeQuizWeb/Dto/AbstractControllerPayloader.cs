using Microsoft.AspNetCore.Mvc;

namespace CollegeQuizWeb.Dto;

public abstract class AbstractControllerPayloader<T> where T: Controller
{
    private T _controllerReference;

    protected AbstractControllerPayloader(T controllerReference)
    {
        _controllerReference = controllerReference;
    }

    public T ControllerReference { get => _controllerReference; }
}