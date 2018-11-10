﻿using IgOutlook.Business.Calendar;
using IgOutlook.Business.Mail;
using IgOutlook.Services.Properties;
using IgOutlook.Services.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace IgOutlook.Services
{
    public class MailService : IMailService
    {
        #region Methods

        public IList<MailMessage> GetInboxItems()
        {
            return _inboxItems;
        }

        public async Task<IList<MailMessage>> GetInboxItemsAsync()
        {
            Task<IList<MailMessage>> task = Task<IList<MailMessage>>.Factory.StartNew(() =>
            {
                return GetInboxItems();
            });

            return await task;
        }

        public IList<MailMessage> GetSentItems()
        {
            return _sentItems;
        }

        public async Task<IList<MailMessage>> GetSentItemsAsync()
        {
            Task<IList<MailMessage>> task = Task<IList<MailMessage>>.Factory.StartNew(() =>
            {
                return GetSentItems();
            });

            return await task;
        }

        public IList<MailMessage> GetDeletedItems()
        {
            return _deletedItems;
        }

        public async Task<IList<MailMessage>> GetDeletedItemsAsync()
        {
            Task<IList<MailMessage>> task = Task<IList<MailMessage>>.Factory.StartNew(() =>
            {
                return GetDeletedItems();
            });

            return await task;
        }

        public IList<MailMessage> GetDraftItems()
        {
            return _draftItems;
        }

        public async Task<IList<MailMessage>> GetDraftItemsAsync()
        {
            Task<IList<MailMessage>> task = Task<IList<MailMessage>>.Factory.StartNew(() =>
            {
                return GetDraftItems();
            });

            return await task;
        }

        public async Task<MailMessage> GetMessageByIdAsync(string messageId)
        {
            Task<MailMessage> task = Task<MailMessage>.Factory.StartNew(() =>
            {
                List<MailMessage> allMailItems = new List<MailMessage>();
                allMailItems.AddRange(GetInboxItems());
                allMailItems.AddRange(GetSentItems());
                allMailItems.AddRange(GetDeletedItems());
                return allMailItems.FirstOrDefault(x => x.Id == messageId);
            });

            return await task;
        }

        public async void ReplyToMessage(MailMessage message, string sourceMessageId)
        {
            var sourceMessage = await GetMessageByIdAsync(sourceMessageId);
            sourceMessage.MarkAsReplied();
            SendMessage(message);
        }

        public async void ForwardMessage(MailMessage message, string sourceMessageId)
        {
            var sourceMessage = await GetMessageByIdAsync(sourceMessageId);
            sourceMessage.MarkAsForwarded();
            SendMessage(message);
        }

        public void SendMessage(MailMessage message)
        {
            if ((message.Flags & MailFlags.Draft) == MailFlags.Draft)
                message.Flags &= ~MailFlags.Draft; //remove the draft flag if it was sent

            message.DateSent = DateTime.Now;
            message.Flags = MailFlags.Seen;
            _sentItems.Add(message);
        }

        public void DeleteMessage(MailMessage message, bool permanent = false)
        {
            message.Flags |= MailFlags.Deleted; // marke the message as deleted

            //first simuate deleting the item by moving it to the deleted folders
            if (!_deletedItems.Contains(message))
                _deletedItems.Add(message);

            if (_inboxItems.Contains(message))
                _inboxItems.Remove(message);

            if (_draftItems.Contains(message))
                _draftItems.Remove(message);

            if (_sentItems.Contains(message))
                _sentItems.Remove(message);

            if (permanent)
            {
                if (_deletedItems.Contains(message))
                    _deletedItems.Remove(message);
            }
        }

        public void SaveDraft(MailMessage message)
        {
            message.Flags |= MailFlags.Draft; //add the draft flag

            if (!_draftItems.Contains(message))
                _draftItems.Add(message);
        }

        #endregion //Methods

        #region Data

        private static IList<MailMessage> _deletedItems = new List<MailMessage>();
        private static IList<MailMessage> _draftItems = new List<MailMessage>();

        private static IList<MailMessage> _sentItems = new List<MailMessage>()
        {
            new MailMessage() 
            {
                Id = "1", 
                Body = ServiceResources.DavidSmit_SampleCoverLetterEmail,
                To = new ObservableCollection<string>() {"janem@demo.infragistics.com", "barbarab@demo.infragistics.com", "derekh@demo.infragistics.com" }, 
                From = "davids@demo.infragistics.com", 
                Subject = ServiceResources.DavidSmit_SampleCoverLetterEmail_Subject, 
                DateSent = DateTime.Now, 
                Importance = MailPriority.High, 
                Flags = MailFlags.Seen 
            },

            new MailMessage() {
                Id = "2", 
                Body = ServiceResources.DavidSmit_GraphicDesignerCoverLetter, 
                To = new ObservableCollection<string>() { "janem@demo.infragistics.com", "barbarab@demo.infragistics.com", "maryh@demo.infragistics.com"},
                Cc = new ObservableCollection<string>() { "margaretj@demo.infragistics.com" }, 
                From = "davids@demo.infragistics.com", 
                Subject = ServiceResources.DavidSmit_GraphicDesignerCoverLetter_Subject,
                DateSent = DateTime.Now.AddDays(-2),
                Importance = MailPriority.Normal,
                Flags = MailFlags.Seen, 
                Category = new ActivityCategory()
                {
                    Color = System.Windows.Media.Colors.Red,
                    Description = ResourceStrings.RedCatDesc,
                    CategoryName = ResourceStrings.RedCatName
                }
            },
        };

        private static IList<MailMessage> _inboxItems = new List<MailMessage>()
        {
             new MailMessage() {
                Id = "3", 
                Body = ServiceResources.BarbaraBailey_RE_SampleCoverLetterEmail, 
                To = new ObservableCollection<string>() { "davids@demo.infragistics.com" },
                Cc = new ObservableCollection<string>() { "bobbyj@demo.infragistics.com", "maryh@demo.infragistics.com" }, 
                From = "barbarab@demo.infragistics.com", 
                Subject = ServiceResources.BarbaraBailey_RE_SampleCoverLetterEmail_Subject,
                DateSent = DateTime.Now,
                Importance = MailPriority.Normal, 
                Flags = MailFlags.Seen, 
                Category = new ActivityCategory()
                {
                    Color = System.Windows.Media.Colors.Red,
                    Description = ResourceStrings.RedCatDesc,
                    CategoryName = ResourceStrings.RedCatName
                }
             },

             new MailMessage() {
                Id = "4", 
                Body = ServiceResources.Barbara_Bailey_RE_GraphicDesignerCoverLetter, 
                To = new ObservableCollection<string>() { "davids@demo.infragistics.com" },
                Cc = new ObservableCollection<string>() { "janem@demo.infragistics.com", "margaretj@demo.infragistics.com", "maryh@demo.infragistics.com" }, 
                From = "barbarab@demo.infragistics.com", 
                Subject = ServiceResources.Barbara_Bailey_RE_GraphicDesignerCoverLetter_Subject,
                DateSent = DateTime.Now.AddDays(-1),
                Importance = MailPriority.Normal, 
                Flags = MailFlags.Flagged | MailFlags.Seen },

             new MailMessage() {
                Id = "5", 
                Body = ServiceResources.MargaretJones_RE_GraphicDesignerCoverLetter, 
                To = new ObservableCollection<string>() { "davids@demo.infragistics.com" },
                Cc = new ObservableCollection<string>() { "janem@demo.infragistics.com", "barbarab@demo.infragistics.com", "maryh@demo.infragistics.com" }, 
                From = "margaretj@demo.infragistics.com", 
                Subject = ServiceResources.Barbara_Bailey_RE_GraphicDesignerCoverLetter_Subject,
                DateSent = DateTime.Now,
                Importance = MailPriority.Normal },
        };

        #endregion //Data
    }
}
