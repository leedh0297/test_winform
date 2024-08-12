using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace testAPP
{
    public partial class Form1 : Form
    {
        private List<string[]> data = new List<string[]>(); // 데이터를 저장할 리스트
        private List<string> changes = new List<string>(); // 변경 내역을 저장할 리스트
        private SortOrder sortOrder = SortOrder.Ascending; // 정렬 순서
        private int sortColumn = -1; // 정렬할 컬럼의 인덱스
        private int selectedIndex = -1; // 선택된 항목의 인덱스를 저장할 변수
        private int tempIdCounter = 1; // 임시 ID를 위한 카운터
        private bool isUpdatingFromListView = false; // 리스트뷰에서 텍스트박스를 업데이트 중인지 여부

        private string connectionString = "Host=192.168.201.151;Username=postgres;Password=12345678;Database=internTest"; // PostgreSQL 연결 문자열

        public Form1()
        {
            InitializeComponent();

            // ListView의 열을 동적으로 설정
            SetupListViewColumns();

            // 콤보박스 초기화
            SetupComboBox();

            // 데이터베이스에서 데이터 로드
            LoadDataFromDatabase();

            // 전체 도서 수 표시
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // ColumnClick 이벤트 핸들러 추가
            lv_list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);

            // SelectedIndexChanged 이벤트 핸들러 추가
            lv_list.SelectedIndexChanged += new EventHandler(lv_list_SelectedIndexChanged);

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터를 불러왔습니다.";

            // 텍스트박스 TextChanged 이벤트 핸들러 추가
            tb_title.TextChanged += new EventHandler(tb_TextChanged);
            tb_writer.TextChanged += new EventHandler(tb_TextChanged);
            tb_genre.TextChanged += new EventHandler(tb_TextChanged);
            tb_description.TextChanged += new EventHandler(tb_TextChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // 테이블의 칼럼을 ListView에 동적으로 추가
        private void SetupListViewColumns()
        {
            lv_list.Columns.Clear();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            string dataType = reader["data_type"].ToString();

                            // 열 너비를 데이터 유형에 따라 결정 (여기서는 임의로 설정)
                            int columnWidth = dataType == "text" ? 300 : 100;

                            // 열 추가
                            lv_list.Columns.Add(columnName, columnWidth);
                        }
                    }
                }
            }

            lv_list.View = View.Details; // ListView를 상세보기로 설정
        }

        // 데이터베이스에서 데이터 로드
        private void LoadDataFromDatabase()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM book", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        data.Clear(); // 기존 데이터를 지움
                        while (reader.Read())
                        {
                            string[] row = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i].ToString();
                            }
                            data.Add(row); // 새 데이터를 추가
                        }
                    }
                }
            }
            UpdateListView(); // ListView를 업데이트
        }

        // 검색 버튼 클릭 시
        private void search_Click(object sender, EventArgs e)
        {
            string searchText = tb_search.Text.ToLower(); // 검색어를 소문자로 변환
            string selectedColumn = cb_filter.SelectedItem.ToString(); // 선택된 콤보박스 항목
            UpdateListView(selectedColumn, searchText); // ListView를 업데이트
        }

        // 임시 ID 생성 함수
        private string GenerateTempId()
        {
            // A, B, C 등의 임시 ID를 생성
            return (tempIdCounter++ + "*").ToString();
        }

        // 리스트 칼럼 클릭 시 정렬
        private void list_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
            {
                // 기존칼럼을 눌렀을때 정렬 방향을 변경
                sortOrder = (sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // 새로운 칼럼을 오름차순 정렬
                sortColumn = e.Column;
                sortOrder = SortOrder.Ascending;
            }

            // list뷰 정렬
            lv_list.ListViewItemSorter = new ListViewItemComparer(e.Column, sortOrder);
        }

        // ListViewItemComparer 클래스: ListView 정렬을 위한 클래스
        public class ListViewItemComparer : IComparer
        {
            private int col; // 정렬할 컬럼
            private SortOrder order; // 정렬 순서

            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                int returnVal;
                if (col == 0) // 첫 번째 열 (id 열)은 숫자형으로 정렬
                {
                    bool isTempId1 = !int.TryParse(((ListViewItem)x).SubItems[col].Text, out int id1);
                    bool isTempId2 = !int.TryParse(((ListViewItem)y).SubItems[col].Text, out int id2);

                    if (isTempId1 && isTempId2)
                    {
                        returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                    }
                    else if (isTempId1)
                    {
                        returnVal = 1; // Temp ID goes after real ID
                    }
                    else if (isTempId2)
                    {
                        returnVal = -1; // Real ID goes before temp ID
                    }
                    else
                    {
                        returnVal = id1.CompareTo(id2);
                    }
                }
                else
                {
                    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                               ((ListViewItem)y).SubItems[col].Text);
                }

                if (order == SortOrder.Descending)
                    returnVal *= -1;

                return returnVal;
            }
        }

        // 삽입 버튼 클릭 시
        private async void insert_Click(object sender, EventArgs e)
        {
            string title = tb_title.Text.Trim();
            string writer = tb_writer.Text.Trim();
            string genre = tb_genre.Text.Trim();
            string description = tb_description.Text.Trim();

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(writer) &&
                string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(description))
            {
                MessageBox.Show("모든 항목이 빈칸일 수 없습니다.");
                return;
            }

            string tempId = GenerateTempId();

            // Create SQL insert query using parameters
            string query = "INSERT INTO book (title, writer, genre, description) VALUES (@Title, @Writer, @Genre, @Description)";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Title", title);
                    cmd.Parameters.AddWithValue("Writer", writer);
                    cmd.Parameters.AddWithValue("Genre", genre);
                    cmd.Parameters.AddWithValue("Description", description);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            //ListView 업데이트
            string[] row = new string[] { tempId, title, writer, genre, description };
            data.Add(row);
            UpdateListView();

            //텍스트박스 초기화
            total_books.Text = "전체 도서수 : " + data.Count.ToString();
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";

            results_report2.Text = "데이터가 삽입되었습니다.";
            ResetLabels();
        }


        // 수정 버튼 클릭 시
        private void bt_edit_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                string id = lv_list.SelectedItems[0].SubItems[0].Text; // 선택된 항목의 ID

                // 입력된 값을 읽음
                string newTitle = tb_title.Text.Trim();
                string newWriter = tb_writer.Text.Trim();
                string newGenre = tb_genre.Text.Trim();
                string newDescription = tb_description.Text.Trim();

                // 기존 값을 읽음
                string oldTitle = lv_list.SelectedItems[0].SubItems[1].Text;
                string oldWriter = lv_list.SelectedItems[0].SubItems[2].Text;
                string oldGenre = lv_list.SelectedItems[0].SubItems[3].Text;
                string oldDescription = lv_list.SelectedItems[0].SubItems[4].Text;

                List<string> updates = new List<string>();

                if (newTitle != oldTitle) updates.Add($"title='{newTitle}'");
                if (newWriter != oldWriter) updates.Add($"writer='{newWriter}'");
                if (newGenre != oldGenre) updates.Add($"genre='{newGenre}'");
                if (newDescription != oldDescription) updates.Add($"description='{newDescription}'");

                if (updates.Count > 0)
                {
                    if (id.Contains("*"))
                    {
                        // 임시 데이터인 경우 기존 임시 데이터를 삭제하고 새 임시 데이터를 추가
                        data.RemoveAt(selectedIndex);
                        lv_list.Items.RemoveAt(selectedIndex);

                        string[] newRow = new string[] { GenerateTempId(), newTitle, newWriter, newGenre, newDescription };
                        data.Add(newRow);
                        UpdateListView();
                    }
                    else
                    {
                        // 데이터베이스에 있는 데이터인 경우 수정 쿼리 생성
                        string updateQuery = $"UPDATE book SET {string.Join(", ", updates)} WHERE id={id}";
                        changes.Add(updateQuery);

                        // ListView 업데이트
                        lv_list.SelectedItems[0].SubItems[1].Text = newTitle;
                        lv_list.SelectedItems[0].SubItems[2].Text = newWriter;
                        lv_list.SelectedItems[0].SubItems[3].Text = newGenre;
                        lv_list.SelectedItems[0].SubItems[4].Text = newDescription;
                    }

                    // 텍스트박스 초기화
                    selectedIndex = -1;
                    tb_title.Text = "";
                    tb_writer.Text = "";
                    tb_genre.Text = "";
                    tb_description.Text = "";

                    // 결과 보고 라벨 업데이트
                    results_report2.Text = "데이터가 수정되었습니다.";
                }
                else
                {
                    MessageBox.Show("변경된 내용이 없습니다.");
                }
            }
            else
            {
                MessageBox.Show("수정할 항목을 선택하세요.");
            }

            // 별표 제거
            ResetLabels();
        }



        // 삭제 버튼 클릭 시
        private void bt_delete_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                string id = lv_list.SelectedItems[0].SubItems[0].Text; // 선택된 항목의 ID

                if (!id.Contains("*"))
                {
                    // 데이터베이스에 있는 데이터인 경우에만 삭제 쿼리를 추가
                    changes.Add($"DELETE FROM book WHERE id={id}");
                }

                // 데이터 삭제
                lv_list.Items.RemoveAt(selectedIndex);
                data.RemoveAt(selectedIndex);

                total_books.Text = "전체 도서수 : " + data.Count.ToString();

                selectedIndex = -1;
                tb_title.Text = "";
                tb_writer.Text = "";
                tb_genre.Text = "";
                tb_description.Text = "";
            }
            else
            {
                MessageBox.Show("삭제할 항목을 선택하세요.");
            }

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터가 삭제되었습니다.";

            // 별표 제거
            ResetLabels();
        }


        // 콤보박스 칼럼 검색
        private void SetupComboBox()
        {
            // 콤보박스 초기화
            cb_filter.Items.Clear();
            cb_filter.Items.Add("전체");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            if (columnName != "id") // 'id' 열은 필터에 포함하지 않음
                            {
                                cb_filter.Items.Add(columnName);
                            }
                        }
                    }
                }
            }

            cb_filter.SelectedIndex = 0; // 기본 설정은 '전체'
        }

        // ListView 업데이트
        private void UpdateListView(string filterColumn = "", string searchText = "")
        {
            lv_list.Items.Clear();

            foreach (string[] row in data)
            {
                bool addRow = true;
                if (!string.IsNullOrEmpty(searchText))
                {
                    addRow = row.Any(col => col.ToLower().Contains(searchText));
                }

                if (addRow && !string.IsNullOrEmpty(filterColumn) && filterColumn != "전체")
                {
                    int columnIndex = lv_list.Columns.Cast<ColumnHeader>().ToList().FindIndex(c => c.Text == filterColumn);
                    if (columnIndex != -1)
                    {
                        addRow = row[columnIndex].ToLower().Contains(searchText);
                    }
                }

                if (addRow)
                {
                    ListViewItem item = new ListViewItem(row);
                    lv_list.Items.Add(item);
                }
            }
        }

        // 리스트뷰 항목 선택 이벤트 핸들러 추가
        private void lv_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_list.SelectedItems.Count > 0)
            {
                isUpdatingFromListView = true; // 리스트뷰에서 업데이트 시작

                selectedIndex = lv_list.SelectedIndices[0]; // 선택된 항목의 인덱스 저장

                // 선택된 항목의 텍스트박스를 업데이트
                tb_title.Text = lv_list.SelectedItems[0].SubItems[1].Text;
                tb_writer.Text = lv_list.SelectedItems[0].SubItems[2].Text;
                tb_genre.Text = lv_list.SelectedItems[0].SubItems[3].Text;
                tb_description.Text = lv_list.SelectedItems[0].SubItems[4].Text;

                isUpdatingFromListView = false; // 리스트뷰에서 업데이트 종료
            }
        }

        // 적용 버튼 클릭 시
        private void bt_apply_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                foreach (string change in changes)
                {
                    using (var cmd = new NpgsqlCommand(change, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            changes.Clear();
            LoadDataFromDatabase();

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터베이스에 저장되었습니다.";

            // 별표 제거
            ResetLabels();
        }

        // 새로고침 버튼 클릭 시
        private void bt_refresh_Click(object sender, EventArgs e)
        {
            // 데이터베이스에서 최신 데이터 로드
            LoadDataFromDatabase();

            // 전체 도서 수 업데이트
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // 텍스트 박스 및 선택된 항목 초기화
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";

            // 선택된 항목 초기화
            selectedIndex = -1;

            // ListView 업데이트
            UpdateListView();

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터를 새로 고침 했습니다.";

            // 별표 제거
            ResetLabels();
        }

        // 수정된 라벨 별표 추가
        private void tb_TextChanged(object sender, EventArgs e)
        {
            if (!isUpdatingFromListView) // 리스트뷰에서 업데이트 중이 아닐 때만 별표 추가
            {
                if (sender == tb_title)
                    title_label.Text = "Title*";
                else if (sender == tb_writer)
                    writer_label.Text = "Writer*";
                else if (sender == tb_genre)
                    genre_label.Text = "Genre*";
                else if (sender == tb_description)
                    description_label.Text = "Description*";
            }
        }

        // 라벨 별표 제거 함수
        private void ResetLabels()
        {
            title_label.Text = "Title";
            writer_label.Text = "Writer";
            genre_label.Text = "Genre";
            description_label.Text = "Description";
        }
    }
}