import React, {useEffect, useLayoutEffect, useState} from "react";
import {SubjectDto} from "../models/Subject";
import StudentAuthService from "../services/studentAuthService";
import {subjectsService} from "../services/subjectsService";
import Subject from "./Subject";
import {Button, Modal} from "react-bootstrap";


const SubjectsLayout=()=> {
    const [loading, setLoading] = useState<boolean>(true)
    const [subjects, setSubjects] = useState<SubjectDto[] | null>(null)
    const [disabledSubjects, setDisabledSubjects] = useState<SubjectDto[] | null>(null)
    const [showModal, setShowModal] = useState<boolean>(false)
    const studentService = new StudentAuthService()
    const [rerender, setRerender] = useState<boolean>(false)
    const loadData = async () => {
        const token = studentService.getToken()!
        const studentId = studentService.getStudent()?.id!
        let subjects: SubjectDto[] | null = null;
        if (studentId === null) {
            console.error("User not logined");
        }
        try {
            subjects = await subjectsService.getSubjects(studentId, token);
        } catch (e) {
            console.error(e)
        }
        setSubjects(subjects)
        setLoading(false)
    }

    const loadChoiseData = async () => {
        const token = studentService.getToken()!
        const studentId = studentService.getStudent()?.id!
        let disabledSubjects: SubjectDto[] | null;
        try {
            disabledSubjects = await subjectsService.getDisabledSubjects(studentId, token)
        } catch (e) {
            console.error(e)
        }

        setDisabledSubjects(disabledSubjects!)
    }
    const handleClose = () => {
        setShowModal(false)
    }
    const handleOpen = () => {
        setShowModal(true)
    }
    // enable subject
    const deleteSubjectFormList = (subjectId: string) => {
        const token = studentService.getToken()!;
        const studentId = studentService.getStudent()?.id!
        subjectsService.enableSubject(subjectId, studentId, token)
            .then(_ => {
                // delete from disabled 
                // add to subjects
                setRerender(!rerender)
            })
            .catch(e => console.error(e))

    }
    // disable subject
    const addSubjectToList = (subjectId: string) => {
        const token = studentService.getToken()!;
        const studentId = studentService.getStudent()?.id!
        subjectsService.disableSubject(subjectId, studentId, token)
            .then(_ => {
                setRerender(!rerender)
            })
            .catch(e => console.error(e))
    }
    useLayoutEffect(() => {
        loadData()
        loadChoiseData()
    }, [rerender])
    
    if (loading) {
        return (
            <div className="spinner-border" role="status">
                <span className="sr-only">Loading...</span>
            </div>
        );
    }

    return (<div className="container">
        <div className="row">
            {
                subjects?.map(s => <Subject subject={s} deleteFunc={addSubjectToList} key={s.id}/>)
            }
        </div>
        {/*{this.renderRedirect()}*/}
        <div className="row">
            <div className="col offset-8"><a className="btn btn-success btn-icon-split" role="button"><span
                className="text-white-50 icon"><i className="fas fa-plus"/></span><span
                className="text-white text" onClick={handleOpen}>Add subject</span></a>
            </div>
            <Modal show={showModal} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Select subjects to add</Modal.Title>
                </Modal.Header>
                <Modal.Body>{disabledSubjects?.map(s =>
                    <button type="button" className="btn btn-primary m-1" onClick={() => {
                        deleteSubjectFormList(s.id)
                    }}>{s.name}</button>
                )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>
        </div>
    </div>)
}

export default SubjectsLayout