import React, {FormEvent} from "react";
import TimetableService from "../services/timrtableService";
import StudentAuthService, {UsageTypes} from "../services/studentAuthService";
import {LessonWithSubject} from "../models/LessonWithSubject";
import Day from "./Day";
import {Button, Modal} from "react-bootstrap";
import {subjectsService} from "../services/subjectsService";
import TelegramLoginForSite from "../Layouts/Login/TelegramLoginForSite";

interface TimetableProps {
    loading: boolean;
    timetable: LessonWithSubject[] | null;
    showModal: boolean,
    lessonToChange: LessonWithSubject | null,
    formValue: string | null,
    showAlert: boolean
}

class Timetable extends React.Component {
    // Todo in table breaks when screen size < xl
    // Todo items for first lesson donit showing 
    constructor(props: any) {
        super(props);
    }

    state: TimetableProps = {
        loading: true,
        timetable: null,
        showModal: false,
        lessonToChange: null,
        formValue: null,
        showAlert: false
    };

    async loadData() {
        let timetable: LessonWithSubject[] | null = null;
        const studentService = new StudentAuthService()
        const usageType = studentService.getUsageType()
        try {
            if (usageType === UsageTypes.Authentificated) {
                const student_id = studentService.getStudent()?.id
                const token = studentService.getStudent()?.token
                timetable = await TimetableService.getTimeTable(student_id!, token!)
            } else if (usageType === UsageTypes.ByGroup) {
                const group_Id = studentService.getGroup()!
                timetable = await TimetableService.getGroupTimeTable(group_Id)
            }
        } catch (e) {
            console.error(e);
        }
        return timetable;
    }

    // todo add icon with group and name
    componentDidMount() {
        this.loadData()
            .then((data) =>
                this.setState({
                    loading: false,
                    timetable: data,
                })
            )
            .catch(e => console.error(e));
    }

    handleClose = () => {
        this.setState({
            showModal: false
        })
    }
    handleCloseAlert = () => {
        this.setState({
            showAlert: false
        })
    }
    handleOpenAlert = () => {
        this.setState({
            showAlert: true
        })
    }
    handleOpen = () => {

        this.setState({
            showModal: true
        })
    }

    handleChange = (event: any) => {
        this.setState({formValue: event.target.value});
    }

    showModal = (lesson: LessonWithSubject) => {
        const studentService = new StudentAuthService()
        const usageType = studentService.getUsageType()
        if (usageType !== UsageTypes.Authentificated) {
            // todo add ability to autentificate rigth here!
            this.handleOpenAlert()
            return
        }
        this.setState({
            lessonToChange: lesson
        });
        this.handleOpen()
    }
    onSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        const link = this.state.formValue
        const lesson = this.state.lessonToChange
        const studentService = new StudentAuthService()
        const token = studentService.getToken()
        subjectsService.updateSubjectLinks(lesson?.type!, link!, lesson?.subject.id!, token!)
            .then(() => {
                let timetable = this.state.timetable!
                if(lesson?.type=="Лек") {
                    timetable!.find(l => l.subject.id === lesson?.subject.id!)!.subject.lessonsZoom = link!
                }
                else if(lesson?.type=="Лаб" ||lesson?.type=="Прак" ) {
                    timetable!.find(l => l.subject.id === lesson?.subject.id!)!.subject.labsZoom = link!
                }
                this.setState({formValue: "", lessonToChange: null, timetable :timetable });
            })
            .catch(e => alert(e));
        this.handleClose()
        this.forceUpdate()
    }

    render() {
        if (this.state.loading) {
            return (
                <div className="spinner-border" role="status">
                    <span className="sr-only">Loading...</span>
                </div>
            );
        }

        if (!this.state.loading) {
            const firstWeek:
                | LessonWithSubject[]
                | undefined = this.state.timetable?.filter((l) => l.week === 1);
            const secondWeek:
                | LessonWithSubject[]
                | undefined = this.state.timetable?.filter((l) => l.week === 2);
            return (
                <div>
                    <h1>Перший тиждень</h1>
                    <Modal show={this.state.showModal} onHide={this.handleClose}>
                        <Modal.Header closeButton>
                            <Modal.Title>Вставте оновлене посилання</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <form onSubmit={this.onSubmit}>
                                <label>
                                    Посилання
                                    <input type="text" className="input-group" value={this.state.formValue!}
                                           onChange={this.handleChange}/>
                                </label>
                                <Button variant="primary" type="submit">
                                    Send
                                </Button>
                            </form>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={this.handleClose}>
                                Close
                            </Button>
                        </Modal.Footer>
                    </Modal>
                    <Modal show={this.state.showAlert} onHide={this.handleCloseAlert}>
                        <Modal.Header closeButton>
                            <Modal.Title>Помилка</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <p>Для доступу до цієї функції потрібна авторизація !</p>
                            <TelegramLoginForSite closeFunc={this.handleCloseAlert}/>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={this.handleCloseAlert}>
                                Закрити
                            </Button>
                        </Modal.Footer>
                    </Modal>
                    <div className="container">
                        <div className="row">
                            <Day
                                key={1}
                                nameOfDay={"Понеділок"}
                                lessons={firstWeek?.filter((l) => l.dayOfWeek === 1)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={2}
                                nameOfDay={"Вівторок"}
                                lessons={firstWeek?.filter((l) => l.dayOfWeek === 2)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={3}
                                nameOfDay={"Середа"}
                                lessons={firstWeek?.filter((l) => l.dayOfWeek === 3)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={4}
                                nameOfDay={"Четвер"}
                                lessons={firstWeek?.filter((l) => l.dayOfWeek === 4)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={5}
                                nameOfDay={"П'ятниця"}
                                lessons={firstWeek?.filter((l) => l.dayOfWeek == 5)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={6}
                                nameOfDay={"Субота"}
                                lessons={firstWeek?.filter((l) => l.dayOfWeek === 6)!}
                                editFunc={this.showModal}
                            />
                        </div>
                    </div>
                    <h1>Другий тиждень</h1>
                    <div className="container">
                        <div className="row">
                            <Day
                                key={7}
                                nameOfDay={"Понеділок"}
                                lessons={secondWeek?.filter((l) => l.dayOfWeek === 1)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={8}
                                nameOfDay={"Вівторок"}
                                lessons={secondWeek?.filter((l) => l.dayOfWeek === 2)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={9}
                                nameOfDay={"Середа"}
                                lessons={secondWeek?.filter((l) => l.dayOfWeek === 3)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={10}
                                nameOfDay={"Четвер"}
                                lessons={secondWeek?.filter((l) => l.dayOfWeek === 4)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={11}
                                nameOfDay={"П'ятниця"}
                                lessons={secondWeek?.filter((l) => l.dayOfWeek == 5)!}
                                editFunc={this.showModal}
                            />
                            <Day
                                key={12}
                                nameOfDay={"Субота"}
                                lessons={secondWeek?.filter((l) => l.dayOfWeek === 6)!}
                                editFunc={this.showModal}
                            />
                        </div>
                    </div>
                </div>
            );
        }
    }
}

export default Timetable;
