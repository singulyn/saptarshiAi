namespace SaptariX.Modules.UIKit.Contracts;

public sealed record UiComponentPageDto(
    string Title,
    string Kicker,
    string Summary,
    string ActiveRoute,
    IReadOnlyList<UiComponentCardDto> Cards,
    IReadOnlyList<UiComponentExampleDto> Examples);
