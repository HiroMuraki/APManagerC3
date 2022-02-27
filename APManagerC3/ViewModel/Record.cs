using System;
using System.Collections.Generic;
using System.IO;

namespace APManagerC3.ViewModel {
    public class Record : ViewModelBase, IViewModel<Model.Record, Record> {
        public string Label {
            get {
                return _label;
            }
            set {
                _label = value;
                OnPropertyChanged(nameof(Label));
                APManager.SaveRequired = true;
            }
        }
        public string Information {
            get {
                return _information;
            }
            set {
                _information = value;
                OnPropertyChanged(nameof(Information));
                APManager.SaveRequired = true;
            }
        }

        public Record GetDeepCopy() {
            Record copy = new Record();
            copy._label = _label;
            copy._information = _information;
            return copy;
        }
        public static List<Record> GetRecordsByFile(string filePath) {
            List<Record> output = new List<Record>();
            using (StreamReader reader = new StreamReader(filePath)) {
                while (!reader.EndOfStream) {
                    string? currentLine = reader.ReadLine();
                    if (string.IsNullOrEmpty(currentLine)) {
                        continue;
                    }
                    output.Add(ResolveText(currentLine));
                }
            }
            return output;
        }
        public static List<Record> GetRecordsByText(string text) {
            List<Record> output = new List<Record>();
            var lines = text.Split(Environment.NewLine);
            foreach (var currentLine in lines) {
                if (string.IsNullOrEmpty(currentLine)) {
                    continue;
                }
                output.Add(ResolveText(currentLine));
            }
            return output;
        }

        private static readonly char[] _delimiters = new char[2] { ':', '：' };
        private string _label = "";
        private string _information = "";
        private static Record ResolveText(string currentLine) {
            Record record = new Record();
            var data = currentLine.Split(_delimiters, 2);
            if (data.Length >= 2) {
                record._label = data[0];
                record._information = data[1];
            } else {
                record._label = currentLine;
                record._information = currentLine;
            }
            return record;
        }

        public void LoadFromModel(Model.Record model) {
            Label = model.Label;
            Information = model.Information;
        }
        public Model.Record ConvertToModel() {
            return new Model.Record() {
                Label = _label,
                Information = _information
            };
        }
    }
}
