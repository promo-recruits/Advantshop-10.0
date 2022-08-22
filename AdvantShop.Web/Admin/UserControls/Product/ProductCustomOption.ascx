<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Admin.UserControls.Products.ProductCustomOption" EnableViewState="true" CodeBehind="ProductCustomOption.ascx.cs" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<adv:EnumDataSource runat="server" ID="edsCustomOptionInputTypes" EnumTypeName="AdvantShop.Catalog.CustomOptionInputType">
</adv:EnumDataSource>
<div style="display: none">
    <asp:Button runat="server" ID="btnSave" OnClick="btnSave_OnClick" Text="Сохранить доп. опции" CssClass="btn btn-middle btn-add save-customoptions" />
</div>
<table class="table-p">
    <tbody>
        <tr id="trHead" runat="server">
            <td class="formheader">
                <h2>Дополнительные опции</h2>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="custom-options">
                    <asp:UpdatePanel ID="UpdatePanelCustomOptions" runat="server" ChildrenAsTriggers="true"
                        UpdateMode="Always">
                        <ContentTemplate>
                            <asp:Repeater ID="rCustomOptions" runat="server" OnItemCommand="rCustomOptions_ItemCommand"
                                OnItemDataBound="rCustomOptions_ItemDataBound">
                                <ItemTemplate>
                                    <div class="option-box">
                                        <asp:HiddenField ID="hfId" runat="server" Value='<%# Eval("CustomOptionsId") %>' />
                                        <asp:HiddenField ID="hfProductId" runat="server" Value='<%# Eval("ProductId") %>' />
                                        <div style="float: right;">
                                            <asp:Button ID="Button1" runat="server" CssClass="btn btn-middle btn-add" CommandArgument='<%# Eval("CustomOptionsId") %>'
                                                CommandName="DeleteCustomOptions" Text="Удалить опцию"
                                                EnableViewState="false" />
                                        </div>
                                        <table class="custom-options-param-table">
                                            <tr class="rowsPost row-interactive">
                                                <td style="width: 150px;">
                                                    Название<span class="required">&nbsp;*</span>
                                                </td>
                                                <td>
                                                    <asp:Panel ID="pInvalidTitle" Visible="false" CssClass="validation-advice" runat="server" EnableViewState="false">
                                                        Поле обязательно для заполнения
                                                    </asp:Panel>
                                                    <asp:TextBox ID="txtTitle" CssClass="niceTextBox shortTextBoxClass" runat="server" Text='<%# Eval("Title") %>' />
                                                </td>
                                            </tr>
                                            <tr class="rowsPost row-interactive">
                                                <td>
                                                    Обязательный
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="cbIsRequired" runat="server" Checked='<%# Eval("IsRequired") %>' />
                                                </td>
                                            </tr>
                                            <tr class="rowsPost row-interactive">
                                                <td>
                                                    Порядок сортировки
                                                </td>
                                                <td>
                                                    <asp:Panel ID="pInvalidSortOrder" Visible="false" CssClass="validation-advice" runat="server" EnableViewState="false">
                                                        Введите корректное число
                                                    </asp:Panel>
                                                    <asp:TextBox ID="txtSortOrder" runat="server" CssClass="niceTextBox shortTextBoxClassX" Text='<%# SQLDataHelper.GetInt( Eval("SortOrder") ) %>'></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr class="rowsPost row-interactive">
                                                <td>
                                                    Тип ввода<span class="required">&nbsp;*</span>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlInputType" runat="server" DataSourceID="edsCustomOptionInputTypes"
                                                        AutoPostBack="true" OnSelectedIndexChanged="ddlInputType_SelectedIndexChanged"
                                                        DataTextField="LocalizedName" DataValueField="Value">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel ID="Table1" runat="server" Visible='<%# (CustomOptionInputType)Eval("InputType") == CustomOptionInputType.CheckBox %>' EnableViewState="false">
                                            <table style="display: inline-table;" class="custom-options-param-table">
                                                <tr class="rowsPost row-interactive">
                                                    <td style="width: 150px;">
                                                        Цена<span class="required">&nbsp;*</span>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtBasePrice" runat="server" CssClass="niceTextBox shortTextBoxClass"
                                                            Text='<%#(CustomOptionInputType)Eval("InputType") == CustomOptionInputType.CheckBox 
                                                                                                                ? (
                                                                                                                 ((List<OptionItem>) Eval("Options")).Count > 0 
                                                                                                                    ? ( (List<OptionItem>) Eval("Options") )[0].BasePrice.ToString("#0.##") 
                                                                                                                    : string.Empty
                                                                                                                )
                                                                                                                : string.Empty  %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr class="rowsPost row-interactive">
                                                    <td>
                                                        Тип цены<span class="required">&nbsp;*</span>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlPriceType" runat="server" EnableViewState="true">
                                                            <asp:ListItem Text="Фиксированная" Value="Fixed"> </asp:ListItem>
                                                            <asp:ListItem Text="Процент" Value="Percent"> </asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <asp:Panel ID="Panel1" runat="server" EnableViewState="false"
                                            Visible='<%# (CustomOptionInputType)Eval("InputType") != CustomOptionInputType.CheckBox && (CustomOptionInputType)Eval("InputType") != CustomOptionInputType.TextBoxMultiLine && (CustomOptionInputType)Eval("InputType") != CustomOptionInputType.TextBoxSingleLine %>'>
                                            <br />
                                            <div style="font-weight: bold; margin-bottom: 5px;">Таблица значений</div>
                                        </asp:Panel>
                                        <asp:GridView ID="grid" runat="server" AutoGenerateColumns="false" OnRowDeleting="grid_RowDeleting"
                                            Visible='<%# (CustomOptionInputType)Eval("InputType") != CustomOptionInputType.CheckBox && (CustomOptionInputType)Eval("InputType") != CustomOptionInputType.TextBoxMultiLine && (CustomOptionInputType)Eval("InputType") != CustomOptionInputType.TextBoxSingleLine %>'
                                            DataSource='<%# Eval("Options") %>' OnRowDataBound="grid_RowDataBound" CssClass="optiontable tableview2">
                                            <Columns>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lId" runat="server" Text='<%# Eval("OptionId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Название
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtTitle" runat="server" Text='<%# SQLDataHelper.GetString(Eval("Title")) %>'
                                                            CssClass="niceTextBox shortTextBoxClassX" Width="165px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Цена
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtBasePrice" runat="server" Text='<%# SQLDataHelper.GetFloat(Eval("BasePrice")).ToString("#0.##")  %>'
                                                            CssClass="niceTextBox shortTextBoxClassX" Width="125px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Тип цены
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlPriceType" runat="server">
                                                            <asp:ListItem Text="Фиксированная" Value="Fixed" />
                                                            <asp:ListItem Text="Процент" Value="Percent" />
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        Сортировка
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtSortOrder" runat="server" Text='<%#SQLDataHelper.GetInt( Eval("SortOrder")) %>'
                                                            CssClass="niceTextBox shortTextBoxClassX" Width="105px" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbDelete" runat="server" OnClientClick="removeunloadhandler();"
                                                            CommandName="Delete" CssClass="Link" Text="Удалить"></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <HeaderStyle ForeColor="Black" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <div class="a-right" style="margin-top: 5px;">
                                            <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-middle btn-action" CommandArgument='<%# Eval("CustomOptionsId") %>'
                                                CommandName="AddNewOption" Visible='<%# ((int)Eval("InputType") == 0 ||(int) Eval("InputType") == 1) %>'
                                                Text="Добавить новую запись" />
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <div style="margin-top: 10px;">
                                <asp:Button CssClass="btn btn-middle btn-add" ID="btnAddCustomOption" runat="server"
                                    Text="Добавить новую опцию" OnClick="btnAddCustomOption_Click" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr id="trHelp" runat="server">
            <td>
               <div class="dvSubHelp" style="margin-left:10px;">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
                    <a href="http://www.advantshop.net/help/pages/additional-options" target="_blank">Инструкция. Дополнительные опции к товару.</a>
                </div> 
            </td>
        </tr>
    </tbody>
</table>