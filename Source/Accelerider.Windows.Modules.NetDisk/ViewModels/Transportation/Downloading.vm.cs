﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        public ICommand PauseAllCommand { get; }

        public ICommand CancelAllCommand { get; }

        public DownloadingViewModel(IUnityContainer container) : base(container)
        {
            PauseAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => PauseCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);

            CancelAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => CancelCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);

            EventAggregator.GetEvent<DownloadItemsAddedEvent>().Subscribe(
                items => items.ForEach(item =>
                {
                    item.Downloader.Subscribe(notification => { });
                    TransferTasks.Add(item);
                }),
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: true,
                filter: _ => TransferTasks != null);
        }

        protected override ObservableSortedCollection<TransferItem> GetTaskList()
        {
            return new ObservableSortedCollection<TransferItem>(
                AcceleriderUser.GetDownloadItems(),
                (x, y) => x.Transporter.Status - y.Transporter.Status);
        }
    }
}
