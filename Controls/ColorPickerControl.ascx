<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ColorPickerControl.ascx.cs" Inherits="Bitboxx.DNNModules.BBStore.Controls.ColorPickerControl" %>
<div style="display:table-cell"><asp:TextBox runat="server" ID="txtColor" style="width: 65px; min-width:inherit"/></div>
<div style="display:table-cell"><div id="colorSelector" runat="server" style="width: 24px; min-height:24px; height:24px; border:1px solid #ccc; padding:0; margin:0;cursor: pointer" ></div></div>
<div id="colorpickerHolder" runat="server" style="width: 356px; height: 0; overflow: hidden; z-index: 99"></div>

<script type="text/javascript">
    $(document).ready(function () {
        var colorPickerHolder = $("#<%=colorpickerHolder.ClientID%>");
        var colorPickerHolderDiv = $("#<%=colorpickerHolder.ClientID%>>div");
        var colorSelector = $("#<%=colorSelector.ClientID%>");
        var txtColor = $("#<%=txtColor.ClientID%>");
        var colorValue = "#" + txtColor.val();
        
        colorSelector.css('backgroundColor', colorValue);
        colorPickerHolder.ColorPicker({
            flat: true,
            color: colorValue,
            onSubmit: function (hsb, hex, rgb) {
                colorSelector.css('backgroundColor', '#' + hex);
                txtColor.val(hex);
            }
        });
        colorPickerHolderDiv.css('position', 'absolute');
        var width = false;
        colorSelector.bind('click', function () {
            colorPickerHolder.stop().animate({ height: width ? 0 : 173 }, 500);
            width = !width;
        });
        txtColor.bind("keyup", function() {
            colorSelector.css('backgroundColor', '#' + txtColor.val());
            colorPickerHolder.ColorPickerSetColor('#' + txtColor.val());
        });
    });
</script>
