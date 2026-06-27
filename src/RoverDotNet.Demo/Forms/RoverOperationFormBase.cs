using System.ComponentModel;

namespace RoverDotNet.Demo.Forms;

/// <summary>
/// Base class for all Rover operation forms. Provides common infrastructure
/// like the optional profile name property.
/// </summary>
public partial class RoverOperationFormBase : Form
{
    /// <summary>
    /// Gets or sets the optional profile name to use for this operation.
    /// If null or empty, the default profile will be used.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? ProfileName { get; set; }

    protected RoverOperationFormBase()
    {
        InitializeComponent();
    }
}
