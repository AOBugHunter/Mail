using System;
using System.Collections.Generic;
using AOSharp.Core;
using AOSharp.Core.UI;
using AOSharp.Common.GameData;
using AOSharp.Common.SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

namespace Mail
{
    public class Main : AOPluginEntry
    {
        private static int _dupeCount = 0;
        private static int _slotNumber = 0;
        private static int _dupeAmount = 0;
        private static int _value = 0;

        public static double _sentMailDupe;
        public static double _sentMail;

        private static bool _doingAll = false;
        private static bool StartMailDupe = false;

        private static string _charName;

        List<string> _slots = new List<string>();

        List<string> _allSlots = new List<string>()
        {
            "0x21",
            "0x22",
            "0x24",
            "0x25",
            "0x26",
            "0x28",
            "0x2A",
            "0x2B",
            "0x2C",
            "0x2D",
            "0x2E",
            "0x2F",
            "0x12",
            "0x15",
            "0x1B",
            "0x1E",
            "0x1A",
            "0x1C",
            "0x09",
            "0x0A",
            "0x0B",
            "0x0C",
            "0x0D",
            "0x0E",
            "0x37",
            "0x38",
            "0x39"
        };

        public override void Run(string pluginDir)
        {
            try
            {
                Chat.WriteLine("Mail plugin loaded");
                Chat.WriteLine("Syntax;");
                Chat.WriteLine("/mail add 0x1B - for example this is the slot number, can add multiple on one line with spaces.");
                Chat.WriteLine("/mail print - prints the slot numbers.");
                Chat.WriteLine("/mail start Jeff 5 - for example this wil send each slot 5 times to jeff.");
                Chat.WriteLine("Has to be capital first letter on the name.");

                Game.OnUpdate += OnUpdate;

                Chat.RegisterCommand("mail", (string command, string[] param, ChatWindow chatWindow) =>
                {
                    if (param.Length >= 1)
                    {
                        if (param[0] == "add")
                        {
                            for (int i = 1; i < param.Length; i++)
                            {
                                _slots.Add(param[i]);
                                Chat.WriteLine($"Added slot {param[i]}.");
                            }
                        }
                        else if (param[0] == "print")
                        {
                            if (_slots.Count >= 1)
                            {
                                foreach (string slot in _slots)
                                {
                                    Chat.WriteLine($"Slot - {slot}.");
                                }
                            }
                            else
                                Chat.WriteLine($"Empty.");
                        }
                        else if (param[0] == "all")
                        {
                            if (param.Length == 3)
                            {
                                if (_allSlots.Count == 0)
                                {
                                    Chat.WriteLine("No slots in list.");
                                }
                                else
                                {

                                    _doingAll = true;

                                    _charName = param[1];
                                    _dupeAmount = Convert.ToInt32(param[2]);
                                    StartMailDupe = !StartMailDupe;
                                    Chat.WriteLine($"Started");
                                }
                            }
                            else
                                Chat.WriteLine($"Name or amount per slot missing.");
                        }
                        else if (param[0] == "start")
                        {
                            if (param.Length == 3)
                            {
                                if (_slots.Count == 0)
                                {
                                    Chat.WriteLine("No slots in list.");
                                }
                                else
                                {
                                    _charName = param[1];
                                    _dupeAmount = Convert.ToInt32(param[2]);
                                    StartMailDupe = !StartMailDupe;
                                    Chat.WriteLine($"Started");
                                }
                            }
                            else
                                Chat.WriteLine($"Name or amount per slot missing.");
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Chat.WriteLine(e.Message);
            }
        }

        private void OnUpdate(object s, float deltaTime)
        {
            if (StartMailDupe)
            {
                if (Time.NormalTime > _sentMailDupe + 70 && !DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.ComputerLiteracy))
                {
                    //int value = (int)new System.ComponentModel.Int32Converter().ConvertFromString(_slots[_slotNumber]);
                    if (_doingAll)
                        _value = (int)new System.ComponentModel.Int32Converter().ConvertFromString(_allSlots[_slotNumber]);
                    else
                        _value = (int)new System.ComponentModel.Int32Converter().ConvertFromString(_slots[_slotNumber]);

                    if (_value == 0) { return; }

                    Identity dupeItem = new Identity();
                    dupeItem.Type = IdentityType.Inventory;
                    dupeItem.Instance = _value;

                    Network.Send(new MailMessage()
                    {
                        Unknown1 = 06,
                        Recipient = $"{_charName}",
                        Subject = "fsdfsdf",
                        Body = "sdfdddf",
                        Item = dupeItem,
                        Credits = 0,
                        Express = true
                    });

                    Chat.WriteLine("Sent.");
                    _dupeCount++;

                    if (_dupeCount == _dupeAmount)
                    {
                        if (_doingAll && _slotNumber < _allSlots.Count - 1)
                        {
                            _slotNumber++;
                            _dupeCount = 0;
                            _sentMailDupe = Time.NormalTime;
                            return;
                        }
                        else if (!_doingAll && _slotNumber < _slots.Count - 1)
                        {
                            _slotNumber++;
                            _dupeCount = 0;
                            _sentMailDupe = Time.NormalTime;
                            return;
                        }
                        else
                        {
                            _slotNumber = 0;
                            _dupeCount = 0;
                            StartMailDupe = false;
                            Chat.WriteLine("Completed.");
                            return;
                        }
                    }

                    _sentMailDupe = Time.NormalTime;
                    return;
                }
            }
        }

        public override void Teardown()
        {
        }
    }
}
