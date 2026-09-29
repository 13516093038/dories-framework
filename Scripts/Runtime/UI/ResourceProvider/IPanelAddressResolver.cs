using System;

public interface IPanelAddressResolver
{
    IPanelAddress Resolve(Type panelType);
}