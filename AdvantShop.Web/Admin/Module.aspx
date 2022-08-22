<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="Admin.Module" CodeBehind="Module.aspx.cs" MasterPageFile="~/Admin/MasterPageEmpty.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
    <link href="../fonts/fonts.admin.css" rel="stylesheet" type="text/css" />
    <link href="css/new_admin/modules-settings.css" rel="stylesheet" type="text/css" />
    <link href="css/new_admin/modules-settings-in-new-view.css" rel="stylesheet" />
    <script>
        function showElement(span) {
            var method_id = $("input:hidden", $(span)).val();
            location = "module.aspx?module=<%= Server.UrlEncode(Request["module"]) %>&currentcontrolindex=" + method_id + (<%=  !string.IsNullOrEmpty(Request["MasterPageEmpty"]) && Request["MasterPageEmpty"].AsBool() == true ? "true" : "false" %> ? "&MasterPageEmpty=true" : "");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="module-in-new-view">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td style="vertical-align: middle; padding: 0 10px 20px 0;">
                    <div class="module-in-new-view-header">
                        Модуль "<asp:Label ID="lblHead" runat="server" />"
                    </div>
                </td>
                <td id="lnkInstruction_block" runat="server" style="vertical-align: middle; padding: 0 10px 20px; text-align: right;" visible="False">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" Width="18" Style="vertical-align: middle;" />
                    <asp:HyperLink ID="lnkInstruction" runat="server" Target="Blank" Style="vertical-align: middle;"></asp:HyperLink>
                </td>
                <td style="vertical-align: middle; padding: 0 0 20px 10px; width: 180px;">

                    <label class="admin-module-checkbox">
                        <input type="checkbox" id="ckbActiveModule" runat="server" class="ckbActiveModule admin-module-checkbox__input"
                            data-modulestringid="" />
                        <span class="admin-module-checkbox__item admin-module-checkbox__item--on">Включен
                        </span>
                        <span class="admin-module-checkbox__item  admin-module-checkbox__item--off">Выключен
                        </span>
                    </label>
                    <div>
                        <div class="lblDeactivatedAndPayable" style="color: red; display: none;">
                            Модуль деактивирован, но все еще установлен в магазине и списания за него продолжатся.
                            <br />
                            Чтобы полностью отключить модуль, необходимо удалить его из магазина.
                        </div>
                    </div>
                </td>
            </tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td style="vertical-align: top; width: 100%" id="tdModuleSettings" runat="server" visible="True">
                    <table cellpadding="0px" cellspacing="0px" style="width: 100%;">
                        <tr>
                            <td style="vertical-align: top; width: 225px;" id="tdTabsHeader" runat="server">
                                <ul class="tabs" id="tabs-headers">
                                    <asp:Repeater runat="server" ID="rptTabs">
                                        <ItemTemplate>
                                            <li id="Li1" runat="server" onclick="javascript:showElement(this)" class='<%# Container.ItemIndex == CurrentControlIndex ? "selected" : "" %>'>
                                                <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Container.ItemIndex %>' />
                                                <asp:Label ID="Literal4" runat="server" Text='<%# Eval("NameTab") %>' />
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </td>
                            <td class="tabContainer" id="tabs-contents">
                                <asp:Panel ID="pnlBody" runat="server" Style="width: 100%">
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Label runat="server" ID="lblInfo"></asp:Label>
    </div>
</asp:Content>
