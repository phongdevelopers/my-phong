using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CommerceBuilder.Web.UI.WebControls
{
    public class SortedGridView : System.Web.UI.WebControls.GridView
    {
        /// <summary>
        /// Get or Set Image location to be used to display Ascending Sort order.
        /// </summary>
        [
        Description("Image to display for Ascending Sort"),
        Category("Misc"),
        Editor("System.Web.UI.Design.UrlEditor", typeof(System.Drawing.Design.UITypeEditor)),
        DefaultValue(""),
        ]
        public string SortAscImageUrl
        {
            get
            {
                object o = ViewState["SortImageAsc"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortImageAsc"] = value;
            }
        }
        /// <summary>
        /// Get or Set Image location to be used to display Ascending Sort order.
        /// </summary>
        [
        Description("Image to display for Descending Sort"),
        Category("Misc"),
        Editor("System.Web.UI.Design.UrlEditor", typeof(System.Drawing.Design.UITypeEditor)),
        DefaultValue(""),
        ]
        public string SortDescImageUrl
        {
            get
            {
                object o = ViewState["SortImageDesc"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortImageDesc"] = value;
            }
        }

        [Browsable(true)]
        [Description("Default sort expression")]
        [Category("Behavior")]
        public string DefaultSortExpression
        {
            get
            {
                string defaultSortExpression = (string)this.ViewState["DefaultSortExpression"];
                if (defaultSortExpression == null)
                {
                    return string.Empty;
                }
                return defaultSortExpression;
            }
            set
            {
                this.ViewState["DefaultSortExpression"] = value;
            }
        }

        [Browsable(true)]
        [Description("Default sort direction")]
        [Category("Behavior")]
        public SortDirection DefaultSortDirection
        {
            get
            {
                object defaultSortDirection = this.ViewState["DefaultSortDirection"];
                if (defaultSortDirection == null)
                {
                    return SortDirection.Ascending;
                }
                return (SortDirection)defaultSortDirection;
            }
            set
            {
                this.ViewState["DefaultSortDirection"] = value;
            }
        }

        [Category("Behavior")]
        [Themeable(true)]
        [Bindable(BindableSupport.No)]
        public bool ShowWhenEmpty
        {
            get
            {
                if (ViewState["ShowWhenEmpty"] == null)
                    ViewState["ShowWhenEmpty"] = false;

                return (bool)ViewState["ShowWhenEmpty"];
            }
            set
            {
                ViewState["ShowWhenEmpty"] = value;
            }
        }

        private string ActiveSortColumn;
        private SortDirection ActiveSortDirection;

        protected override DataSourceSelectArguments CreateDataSourceSelectArguments()
        {
            DataSourceSelectArguments dataSourceSelectArguments = base.CreateDataSourceSelectArguments();
            //CHECK WHETHER A SORT EXPRESSION IS PRESENT
            if (string.IsNullOrEmpty(dataSourceSelectArguments.SortExpression))
            {
                if (!string.IsNullOrEmpty(this.DefaultSortExpression) && (System.Web.HttpContext.Current != null))
                {
                    dataSourceSelectArguments.SortExpression = this.DefaultSortExpression;
                    if (this.DefaultSortDirection == SortDirection.Descending) dataSourceSelectArguments.SortExpression += " DESC";
                    ActiveSortColumn = this.DefaultSortExpression;
                    ActiveSortDirection = this.DefaultSortDirection;
                }
            }
            else
            {
                ActiveSortColumn = this.SortExpression;
                ActiveSortDirection = this.SortDirection;
            }
            return dataSourceSelectArguments;
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                DisplaySortOrderImages(e.Row);
            }
            base.OnRowCreated(e);
        }

        protected override void OnSorting(GridViewSortEventArgs e)
        {
            if (this.SortExpression == string.Empty)
            {
                if (e.SortExpression == this.DefaultSortExpression)
                {
                    e.SortDirection = (this.DefaultSortDirection == SortDirection.Ascending) ? SortDirection.Descending : SortDirection.Ascending;
                }
            }
            base.OnSorting(e);
        }

        /// <summary>
        ///  Display a graphic image for the Sort Order along with the sort sequence no.
        /// </summary>
        protected void DisplaySortOrderImages(GridViewRow headerRow)
        {
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                TableCell headerCell = headerRow.Cells[i];
                if (headerCell.Controls.Count > 0 && headerCell.Controls[0] is LinkButton)
                {
                    string thisColumn = ((LinkButton)headerCell.Controls[0]).CommandArgument;
                    if (ActiveSortColumn == thisColumn) {
                        string sortImageUrl = (ActiveSortDirection == SortDirection.Ascending) ? SortAscImageUrl : SortDescImageUrl;
                        if (!string.IsNullOrEmpty(sortImageUrl))
                        {
                            Image imgSortDirection = new Image();
                            imgSortDirection.ImageUrl = sortImageUrl;
                            headerCell.Controls.Add(new LiteralControl("&nbsp;"));
                            headerCell.Controls.Add(imgSortDirection);
                        }
                        else
                        {

                            Label lblSortDirection = new Label();
                            lblSortDirection.Font.Size = FontUnit.XSmall;
                            lblSortDirection.Font.Name = "Arial";
                            lblSortDirection.EnableTheming = false;
                            lblSortDirection.Text = ((ActiveSortDirection == SortDirection.Ascending) ? " (asc)" : " (desc)");
                            headerCell.Controls.Add(lblSortDirection);
                        }
                    }
                }
            }
        }

        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
        {
            int numRows = base.CreateChildControls(dataSource, dataBinding);

            //no data rows created, create empty table if enabled
            if (numRows == 0 && ShowWhenEmpty)
            {
                //create table
                Table table = new Table();
                table.ID = this.ID;

                //convert the exisiting columns into an array and initialize
                DataControlField[] fields = new DataControlField[this.Columns.Count];
                this.Columns.CopyTo(fields, 0);

                if (this.ShowHeader)
                {
                    //create a new header row
                    GridViewRow headerRow = base.CreateRow(-1, -1, DataControlRowType.Header, DataControlRowState.Normal);

                    this.InitializeRow(headerRow, fields);
                    table.Rows.Add(headerRow);
                }

                //create the empty row
                GridViewRow emptyRow = new GridViewRow(-1, -1, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);

                TableCell cell = new TableCell();
                cell.ColumnSpan = this.Columns.Count;
                cell.Width = Unit.Percentage(100);
                if (!String.IsNullOrEmpty(EmptyDataText))
                    cell.Controls.Add(new LiteralControl(EmptyDataText));

                if (this.EmptyDataTemplate != null)
                    EmptyDataTemplate.InstantiateIn(cell);

                emptyRow.Cells.Add(cell);
                table.Rows.Add(emptyRow);

                if (this.ShowFooter)
                {
                    //create footer row
                    GridViewRow footerRow = base.CreateRow(-1, -1, DataControlRowType.Footer, DataControlRowState.Normal);

                    this.InitializeRow(footerRow, fields);
                    table.Rows.Add(footerRow);
                }

                this.Controls.Clear();
                this.Controls.Add(table);
            }

            return numRows;
        }
    }
}
