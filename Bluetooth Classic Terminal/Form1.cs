using System.Text;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace Bluetooth_Classic_Terminal
{
    public partial class Form1 : Form
    {
        private BluetoothClient? _bluetoothClient;
        private Stream? _stream;
        private CancellationTokenSource? _readCts;
        private Task? _readTask;
        private bool _isBusy;

        public Form1()
        {
            InitializeComponent();
            Shown += Form1_Shown;
            txtInput.KeyDown += txtInput_KeyDown;
        }

        private bool IsConnected => _bluetoothClient?.Connected == true && _stream != null;

        private async void Form1_Shown(object? sender, EventArgs e)
        {
            await LoadPairedDevicesAsync();
        }

        private async void btnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadPairedDevicesAsync();
        }

        private async Task LoadPairedDevicesAsync()
        {
            if (_isBusy || IsConnected)
            {
                return;
            }

            _isBusy = true;
            btnRefresh.Enabled = false;
            btnConnect.Enabled = false;
            SetStatus("Status: Loading paired devices...");

            try
            {
                BluetoothDeviceInfo[] devices = await Task.Run(() =>
                {
                    using var discoveryClient = new BluetoothClient();
                    IReadOnlyCollection<BluetoothDeviceInfo> discovered = discoveryClient.DiscoverDevices();
                    return discovered
                        .Where(d => d.Authenticated)
                        .OrderBy(d => d.DeviceName)
                        .ThenBy(d => d.DeviceAddress.ToString())
                        .ToArray();
                });

                cmbDevices.BeginUpdate();
                cmbDevices.Items.Clear();
                foreach (BluetoothDeviceInfo device in devices)
                {
                    cmbDevices.Items.Add(new PairedDeviceItem(device));
                }

                cmbDevices.EndUpdate();

                if (cmbDevices.Items.Count > 0)
                {
                    cmbDevices.SelectedIndex = 0;
                    SetStatus($"Status: {cmbDevices.Items.Count} paired device(s) found");
                }
                else
                {
                    SetStatus("Status: No paired Bluetooth Classic devices found");
                }
            }
            catch (Exception ex)
            {
                SetStatus("Status: Failed to load paired devices");
                MessageBox.Show(this, ex.Message, "Device Discovery Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isBusy = false;
                UpdateConnectionUi(IsConnected);
            }
        }

        private async void btnConnect_Click(object? sender, EventArgs e)
        {
            if (_isBusy || IsConnected)
            {
                return;
            }

            if (cmbDevices.SelectedItem is not PairedDeviceItem selectedDevice)
            {
                MessageBox.Show(this, "Select a paired device first.", "No Device Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isBusy = true;
            btnConnect.Enabled = false;
            btnRefresh.Enabled = false;
            SetStatus($"Status: Connecting to {selectedDevice.DeviceName}...");

            BluetoothClient? client = null;
            try
            {
                client = new BluetoothClient();
                await Task.Run(() => client.Connect(selectedDevice.DeviceInfo.DeviceAddress, BluetoothService.SerialPort));

                _bluetoothClient = client;
                _stream = client.GetStream();
                _readCts = new CancellationTokenSource();
                _readTask = Task.Run(() => ReadLoopAsync(_readCts.Token));

                UpdateConnectionUi(true);
                SetStatus($"Status: Connected to {selectedDevice.DeviceName}");
                AppendOutput($"[connected] {selectedDevice}");
            }
            catch (Exception ex)
            {
                client?.Dispose();
                _bluetoothClient = null;
                _stream = null;
                _readCts?.Dispose();
                _readCts = null;
                _readTask = null;
                UpdateConnectionUi(false);
                SetStatus("Status: Connection failed");
                MessageBox.Show(this, ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async void btnDisconnect_Click(object? sender, EventArgs e)
        {
            await DisconnectAsync(userInitiated: true);
        }

        private async Task DisconnectAsync(bool userInitiated)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            CancellationTokenSource? cts = _readCts;
            Task? readTask = _readTask;
            Stream? stream = _stream;
            BluetoothClient? client = _bluetoothClient;

            _readCts = null;
            _readTask = null;
            _stream = null;
            _bluetoothClient = null;

            try
            {
                cts?.Cancel();

                if (stream != null)
                {
                    await stream.DisposeAsync();
                }

                if (readTask != null)
                {
                    try
                    {
                        await readTask;
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            }
            finally
            {
                cts?.Dispose();
                client?.Dispose();
                _isBusy = false;
                UpdateConnectionUi(false);
                SetStatus("Status: Disconnected");
                if (userInitiated)
                {
                    AppendOutput("[disconnected]");
                }
            }
        }

        private async Task ReadLoopAsync(CancellationToken cancellationToken)
        {
            Stream? readStream = _stream;
            if (readStream == null)
            {
                return;
            }

            byte[] buffer = new byte[1024];
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead;
                try
                {
                    bytesRead = await readStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    AppendOutput($"[read error] {ex.Message}");
                    break;
                }

                if (bytesRead == 0)
                {
                    AppendOutput("[remote closed connection]");
                    break;
                }

                string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);

                if (receivedText.Length == 0)
                {
                    continue;
                }

                AppendOutput($"RX: {receivedText}");
            }

            if (!cancellationToken.IsCancellationRequested && !IsDisposed)
            {
                BeginInvoke(new Action(() => _ = DisconnectAsync(userInitiated: false)));
            }
        }

        private async void btnSend_Click(object? sender, EventArgs e)
        {
            await SendAsync();
        }

        private async void txtInput_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            await SendAsync();
        }

        private async Task SendAsync()
        {
            if (!IsConnected || _stream == null)
            {
                return;
            }

            string message = txtInput.Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            byte[] payload = Encoding.UTF8.GetBytes(message + "\r\n");
            try
            {
                await _stream.WriteAsync(payload);
                await _stream.FlushAsync();
                AppendOutput($"TX: {message}");
                txtInput.Clear();
            }
            catch (Exception ex)
            {
                AppendOutput($"[write error] {ex.Message}");
                await DisconnectAsync(userInitiated: false);
            }
        }

        private void AppendOutput(string message)
        {
            if (txtOutput.InvokeRequired)
            {
                txtOutput.BeginInvoke(() => AppendOutput(message));
                return;
            }

            string line = $"{DateTime.Now:HH:mm:ss} {message}";
            txtOutput.AppendText(line + Environment.NewLine);
        }

        private void SetStatus(string statusText)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.BeginInvoke(() => SetStatus(statusText));
                return;
            }

            lblStatus.Text = statusText;
        }

        private void UpdateConnectionUi(bool connected)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateConnectionUi(connected));
                return;
            }

            cmbDevices.Enabled = !connected && !_isBusy;
            btnRefresh.Enabled = !connected && !_isBusy;
            btnConnect.Enabled = !connected && !_isBusy && cmbDevices.Items.Count > 0;
            btnDisconnect.Enabled = connected;
            btnSend.Enabled = connected;
            txtInput.Enabled = connected;
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _readCts?.Cancel();
            _stream?.Dispose();
            _bluetoothClient?.Dispose();
            _readCts?.Dispose();
        }

        private sealed class PairedDeviceItem
        {
            public PairedDeviceItem(BluetoothDeviceInfo deviceInfo)
            {
                DeviceInfo = deviceInfo;
            }

            public BluetoothDeviceInfo DeviceInfo { get; }

            public string DeviceName => string.IsNullOrWhiteSpace(DeviceInfo.DeviceName) ? "(Unnamed Device)" : DeviceInfo.DeviceName;

            public override string ToString() => $"{DeviceName} ({DeviceInfo.DeviceAddress})";
        }
    }
}
