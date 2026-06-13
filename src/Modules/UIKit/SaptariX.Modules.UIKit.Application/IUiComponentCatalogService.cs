using SaptariX.Modules.UIKit.Contracts;

namespace SaptariX.Modules.UIKit.Application;

public interface IUiComponentCatalogService
{
    UiComponentPageDto GetPage(string pageKey);
}
