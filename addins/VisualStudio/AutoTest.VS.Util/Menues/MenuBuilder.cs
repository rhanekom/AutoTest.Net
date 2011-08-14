﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.CommandBars;
using EnvDTE80;
using EnvDTE;

namespace AutoTest.VS.Util.Menues
{
    public class MenuBuilder
    {
        private object[] _contextGUIDS = new object[] { };
        private DTE2 _application;
        private AddIn _addin;

        public MenuBuilder(DTE2 application, AddIn addin)
        {
            _application = application;
            _addin = addin;
        }

        public void AddMenuBar(string name, string caption)
        {
            //Place the command on the tools menu.
            //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
            CommandBar menuBarCommandBar = ((CommandBars)_application.CommandBars)["MenuBar"];
            var commands = (Commands2)_application.Commands;

            CommandBarControl toolsControl = getCommandBarControl(menuBarCommandBar.Controls, name);
            try
            {
                if (toolsControl == null)
                {
                    var command = (CommandBar) commands.AddCommandBar(caption, vsCommandBarType.vsCommandBarTypeMenu, menuBarCommandBar, 30);
                    toolsControl = getCommandBarControl(menuBarCommandBar.Controls, name);
                }
                toolsControl.Enabled = true;
            }
            catch
            {
            }
        }

        private bool menuBarDoesNotExist(string menuName)
        {
            try
            {
                CommandBar menuBarCommandBar = ((CommandBars)_application.CommandBars)["MenuBar"];
                return getCommandBarControl(menuBarCommandBar.Controls, menuName) == null;
            }
            catch
            {
                return true;
            }
        }

        public void CreateMenuItem(string commandBar, string popup, string caption, string description, string bindings, int place, string commandName)
        {
            CreateMenuItem(commandBar, popup, caption, description, bindings, place, commandName, false, 0);
        }

        public bool ContainsControl(CommandBarControls ctrls, string name)
        {
            foreach (var ctrl in ctrls)
                if (((CommandBarControl)ctrl).accName.Equals(name))
                    return true;
            return false;
        }

        public void CreateMenuItem(string commandBar, string popup, string caption, string description, string bindings, int place, string commandName, bool hasSeparator, int icon)
        {
            try
            {
                var commands = (Commands2)_application.Commands;
                var editorCommandBar = ((CommandBars)_application.CommandBars)["MenuBar"];
                var editPopUp = getPopup(editorCommandBar.Controls, popup);
                var item = getCommandBarControl(editPopUp.Controls, caption);
                if (item != null)
                {
                    item.BeginGroup = hasSeparator;
                    return;
                }
                Command command = getCommand(commands, commandName);
                if (command == null)
                    command = commands.AddNamedCommand2(_addin, commandName, caption, description, false, icon, ref _contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported, (int)vsCommandStyle.vsCommandStylePictAndText);
                command.AddControl(editPopUp.CommandBar, place);
                item = getCommandBarControl(editPopUp.Controls, caption);
                item.BeginGroup = hasSeparator;

                if (bindings != null)
                    command.Bindings = bindings;
            }
            catch
            {
            }
        }

        private CommandBar getCommandBar(CommandBars bars, string name)
        {
            var enu = bars.GetEnumerator();

            while (enu.MoveNext())
            {
                var control = (CommandBar)enu.Current;
                if (control.accName == name)
                    return control;
            }
            return null;
        }

        private CommandBarControl getCommandBarControl(CommandBarControls controls, string name)
        {
            var enu = controls.GetEnumerator();

            while (enu.MoveNext())
            {
                var control = (CommandBarControl)enu.Current;
                if (control.accName == name)
                    return control;
            }
            return null;
        }

        private CommandBarPopup getPopup(CommandBarControls controls, string name)
        {
            var enu = controls.GetEnumerator();

            while (enu.MoveNext())
            {
                var control = (CommandBarPopup)enu.Current;
                if (control.accName == name)
                    return control;
            }
            return null;
        }

        private Command getCommand(Commands2 commands, string commandName)
        {
            foreach (Command cmd in commands)
                if (cmd.Name.EndsWith("." + commandName))
                    return cmd;
            return null;
        }

        public CommandBarControl CreateMenuContainer(string commandBar, string popup, string caption, string description, string bindings, int place)
        {
            return CreateMenuContainer(commandBar, popup, caption, description, bindings, place, false, 0);
        }

        public CommandBarControl CreateMenuContainer(string commandBar, string popup, string caption, string description, string bindings, int place, bool hasSeparator, int icon)
        {
            try
            {
                var commands = (Commands2)_application.Commands;
                var editorCommandBar = ((CommandBars)_application.CommandBars)["MenuBar"];
                var editPopUp = getPopup(editorCommandBar.Controls, popup);
                var item = getCommandBarControl(editPopUp.Controls, caption);
                if (item != null)
                {
                    item.BeginGroup = hasSeparator;
                    return item;
                }
                var ctl = editPopUp.Controls.Add(MsoControlType.msoControlPopup, place, null, place, false);
                ctl.Caption = caption;
                ctl.BeginGroup = hasSeparator;
                ctl.Enabled = true;
                return ctl;
            }
            catch
            {
                return null;
            }
        }

        public void CreateSubMenuItem(CommandBarControl parent, string caption, string description, string bindings, int place, string commandName)
        {
            CreateSubMenuItem(parent, caption, description, bindings, place, commandName, false, 0);
        }

        public void CreateSubMenuItem(CommandBarControl parent, string caption, string description, string bindings, int place, string commandName, bool hasSeparator, int icon)
        {
            try
            {
                var commands = (Commands2)_application.Commands;
                var subMenuCtl = (CommandBarPopup)parent;
                var ctl = getCommandBarControl(subMenuCtl.Controls, caption);
                if (ctl != null)
                {
                    ctl.BeginGroup = hasSeparator;
                    return;
                }
                Command command = getCommand(commands, commandName);
                if (command == null)
                    command = commands.AddNamedCommand2(_addin, commandName, caption, description, false, icon, ref _contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported, (int)vsCommandStyle.vsCommandStylePictAndText);
                command.AddControl(subMenuCtl.CommandBar, place);
                ctl = getCommandBarControl(subMenuCtl.Controls, caption);
                ctl.BeginGroup = hasSeparator;

                if (bindings != null)
                    command.Bindings = bindings;
            }
            catch
            {
            }
        }

        public void CleanupOldSubMenuItemByDeletion(CommandBarControl parent, string caption)
        {
            try
            {
                var commands = (Commands2)_application.Commands;
                var subMenuCtl = (CommandBarPopup)parent;
                var ctl = getCommandBarControl(subMenuCtl.Controls, caption);
                if (ctl != null)
                    subMenuCtl.Controls[caption].Delete();
            }
            catch
            {
            }
        }
    }
}