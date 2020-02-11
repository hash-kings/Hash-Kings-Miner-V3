﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using zPoolMiner.Configs;
using zPoolMiner.Devices;
using zPoolMiner.Interfaces;

namespace zPoolMiner.Forms.Components
{
    public partial class DevicesListViewEnableControl : UserControl
    {
        private const int ENABLED = 0;
        private const int DEVICE = 1;

        private class DefaultDevicesColorSeter : IListItemCheckColorSetter
        {
            private static Color ENABLED_COLOR = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(44)))), ((int)(((byte)(51)))));
            private static Color DISABLED_COLOR = Color.DarkGray;

            public void LviSetColor(ListViewItem lvi)
            {
                if (lvi.Tag is ComputeDevice cdvo)
                {
                    if (cdvo.Enabled)
                    {
                        lvi.BackColor = ENABLED_COLOR;
                    }
                    else
                    {
                        lvi.BackColor = DISABLED_COLOR;
                    }
                }
            }
        }

        private IListItemCheckColorSetter _listItemCheckColorSetter = new DefaultDevicesColorSeter();

        public IBenchmarkCalculation BenchmarkCalculation { get; set; }

        private AlgorithmsListView _algorithmsListView = null;

        // disable checkboxes when in benchmark mode
        private bool _isInBenchmark = false;

        // helper for benchmarking logic
        public bool IsInBenchmark
        {
            get { return _isInBenchmark; }
            set
            {
                if (value)
                {
                    _isInBenchmark = value;
                    listViewDevices.CheckBoxes = false;
                }
                else
                {
                    _isInBenchmark = value;
                    listViewDevices.CheckBoxes = true;
                }
            }
        }

        private bool _isMining = false;

        public bool IsMining
        {
            get { return _isMining; }
            set
            {
                if (value)
                {
                    _isMining = value;
                    listViewDevices.CheckBoxes = false;
                }
                else
                {
                    _isMining = value;
                    listViewDevices.CheckBoxes = true;
                }
            }
        }

        public bool IsBenchmarkForm = false;
        public bool IsSettingsCopyEnabled = false;

        public string FirstColumnText
        {
            get { return listViewDevices.Columns[ENABLED].Text; }
            set { if (value != null) listViewDevices.Columns[ENABLED].Text = value; }
        }

        public bool SaveToGeneralConfig { get; set; }

        public DevicesListViewEnableControl()
        {
            InitializeComponent();

            SaveToGeneralConfig = false;
            // intialize ListView callbacks
            listViewDevices.ItemChecked += new ItemCheckedEventHandler(ListViewDevicesItemChecked);
            //listViewDevices.CheckBoxes = false;
            IsMining = false;
            BenchmarkCalculation = null;
        }

        public void SetIListItemCheckColorSetter(IListItemCheckColorSetter listItemCheckColorSetter)
        {
            _listItemCheckColorSetter = listItemCheckColorSetter;
        }

        public void SetAlgorithmsListView(AlgorithmsListView algorithmsListView)
        {
            _algorithmsListView = algorithmsListView;
        }

        public void ResetListItemColors()
        {
            foreach (ListViewItem lvi in listViewDevices.Items)
            {
                if (_listItemCheckColorSetter != null)
                {
                    _listItemCheckColorSetter.LviSetColor(lvi);
                }
            }
        }

        public void SetComputeDevices(List<ComputeDevice> computeDevices)
        {
            // to not run callbacks when setting new
            bool tmp_SaveToGeneralConfig = SaveToGeneralConfig;
            SaveToGeneralConfig = false;
            listViewDevices.BeginUpdate();
            listViewDevices.Items.Clear();
            // set devices
            foreach (var computeDevice in computeDevices)
            {
                ListViewItem lvi = new ListViewItem
                {
                    //lvi.SubItems.Add(computeDevice.Name);
                    Checked = computeDevice.Enabled,
                    Text = computeDevice.GetFullName(),
                    Tag = computeDevice
                };
                listViewDevices.Items.Add(lvi);
                _listItemCheckColorSetter.LviSetColor(lvi);
            }
            listViewDevices.EndUpdate();
            listViewDevices.Invalidate(true);
            // reset properties
            SaveToGeneralConfig = tmp_SaveToGeneralConfig;
        }

        public void ResetComputeDevices(List<ComputeDevice> computeDevices)
        {
            SetComputeDevices(computeDevices);
        }

        public void InitLocale()
        {
            listViewDevices.Columns[ENABLED].Text = International.GetText("ListView_Device");  //International.GetText("ListView_Enabled");
            //listViewDevices.Columns[DEVICE].Text = International.GetText("ListView_Device");
        }

        private void ListViewDevicesItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var CDevice = e.Item.Tag as ComputeDevice;
            CDevice.Enabled = e.Item.Checked;

            if (SaveToGeneralConfig)
            {
                ConfigManager.GeneralConfigFileCommit();
            }
            if (e.Item is ListViewItem lvi) _listItemCheckColorSetter.LviSetColor(lvi);
            if (_algorithmsListView != null) _algorithmsListView.RepaintStatus(CDevice.Enabled, CDevice.UUID);
            if (BenchmarkCalculation != null) BenchmarkCalculation.CalcBenchmarkDevicesAlgorithmQueue();
        }

        public void SetDeviceSelectionChangedCallback(ListViewItemSelectionChangedEventHandler callback)
        {
            listViewDevices.ItemSelectionChanged += callback;
        }

        private void ListViewDevices_MouseClick(object sender, MouseEventArgs e)
        {
            if (IsInBenchmark) return;
            if (IsMining) return;
            if (e.Button == MouseButtons.Right)
            {
                if (listViewDevices.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Items.Clear();
                    if (IsSettingsCopyEnabled)
                    {
                        var CDevice = listViewDevices.FocusedItem.Tag as ComputeDevice;
                        var sameDevTypes = ComputeDeviceManager.Avaliable.GetSameDevicesTypeAsDeviceWithUUID(CDevice.UUID);
                        if (sameDevTypes.Count > 0)
                        {
                            var copyBenchItem = new ToolStripMenuItem();
                            //copyBenchItem.DropDownItems
                            foreach (var cDev in sameDevTypes)
                            {
                                if (cDev.Enabled)
                                {
                                    var copyBenchDropDownItem = new ToolStripMenuItem
                                    {
                                        Text = cDev.Name,
                                        Checked = cDev.UUID == CDevice.BenchmarkCopyUUID
                                    };
                                    copyBenchDropDownItem.Click += ToolStripMenuItemCopySettings_Click;
                                    copyBenchDropDownItem.Tag = cDev.UUID;
                                    copyBenchItem.DropDownItems.Add(copyBenchDropDownItem);
                                }
                            }
                            copyBenchItem.Text = International.GetText("DeviceListView_ContextMenu_CopySettings");
                            contextMenuStrip1.Items.Add(copyBenchItem);
                        }
                    }
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void ToolStripMenuItemCopySettings_Click(object sender, EventArgs e)
        {
            var CDevice = listViewDevices.FocusedItem.Tag as ComputeDevice;
            if (sender is ToolStripMenuItem item)
            {
                if (item.Tag is string uuid)
                {
                    var copyBenchCDev = ComputeDeviceManager.Avaliable.GetDeviceWithUUID(uuid);
                    CDevice.BenchmarkCopyUUID = uuid;

                    var result = MessageBox.Show(
                        String.Format(
                        International.GetText("DeviceListView_ContextMenu_CopySettings_Confirm_Dialog_Msg"), copyBenchCDev.GetFullName(), CDevice.GetFullName()),
                                International.GetText("DeviceListView_ContextMenu_CopySettings_Confirm_Dialog_Title"),
                                MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        // just copy
                        CDevice.CopyBenchmarkSettingsFrom(copyBenchCDev);
                        if (_algorithmsListView != null) _algorithmsListView.RepaintStatus(CDevice.Enabled, CDevice.UUID);
                    }
                }
            }
        }

        private void DevicesListViewEnableControl_Resize(object sender, EventArgs e)
        {
            // only one
            foreach (ColumnHeader ch in listViewDevices.Columns)
            {
                ch.Width = Width - 10;
            }
        }

        public void SetFirstSelected()
        {
            if (listViewDevices.Items.Count > 0)
            {
                listViewDevices.Items[0].Selected = true;
                listViewDevices.Select();
            }
        }
    }
}